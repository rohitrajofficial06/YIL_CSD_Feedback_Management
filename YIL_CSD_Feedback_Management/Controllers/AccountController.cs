using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YIL_CSD_Feedback_Management.Services.Interfaces;
using YIL_CSD_Feedback_Management.ViewModels;

namespace YIL_CSD_Feedback_Management.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        #region Login (GET)

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        #endregion

        #region Login (POST)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _accountService.LoginAsync(model.UserName, model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name or Password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FullName", user.FullName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? ""),
                new Claim("Region", user.Region ?? ""),
                new Claim("Module", user.Module ?? ""),
                new Claim("DepartmentId", user.DepartmentId.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    AllowRefresh = true
                });

            // If user was trying to access a protected page
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) &&
                Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            // Redirect Admins based on Module
            if (string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                switch (user.Module?.Trim().ToLower())
                {
                    case "siteservice":
                        return RedirectToAction(
                            "Index",
                            "Dashboard",
                            new { area = "Admin" });

                    case "benchrepair":
                        // Future Area
                        return RedirectToAction(
                            "Index",
                            "Dashboard",
                            new { area = "BenchRepair" });

                    case "training":
                        // Future Area
                        return RedirectToAction(
                            "Index",
                            "Dashboard",
                            new { area = "Training" });

                    default:
                        TempData["Error"] = "No module has been assigned to your account.";
                        return RedirectToAction(nameof(AccessDenied));
                }
            }

            // Normal Users
            return RedirectToAction("Index", "Dashboard");
        }

        #endregion

        #region Logout

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }

        #endregion

        #region AccessDenied

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion
    }
}