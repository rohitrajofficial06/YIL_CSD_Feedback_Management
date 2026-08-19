using Microsoft.EntityFrameworkCore;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Helpers;
using YIL_CSD_Feedback_Management.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YIL_CSD_Feedback_Management.Services.Interfaces;
using YIL_CSD_Feedback_Management.ViewModels;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ILogService _logService;
        private readonly ApplicationDbContext _context;

        public AccountController(IAccountService accountService, ILogService logService, ApplicationDbContext context)
        {
            _accountService = accountService;
            _logService = logService;
            _context = context;
        }

        #region Login (GET)

        [HttpGet]
        public IActionResult Login(
      string? returnUrl = null,
      bool sessionExpired = false)
        {
            if (sessionExpired)
            {
                TempData["SessionExpired"] =
                    "You have been logged out because there was no activity for 5 minutes.";
            }

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
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                //======================================================
                // Validate Username / Password
                //======================================================

                var user = await _accountService.LoginAsync(
                    model.UserName,
                    model.Password);

                if (user == null)
                {
                    await _logService.WarningAsync(
                        "Account",
                        "Login",
                        $"Invalid login attempt. UserName : {model.UserName}");

                    ModelState.AddModelError(
                        string.Empty,
                        "Invalid User Name or Password.");

                    return View(model);
                }

                //======================================================
                // Check Existing Active Session
                //======================================================

                var existingSession = await _context.UserActiveSessions
                    .FirstOrDefaultAsync(x =>
                        x.UserName == user.UserName &&
                        x.IsActive);

                if (existingSession != null)
                {
                    DateTime currentTime = DateTimeHelper.Now;

                    DateTime sessionExpiry =
                        existingSession.LastActivityDate.AddMinutes(5);

                    //==================================================
                    // Previous session is still active
                    //==================================================

                    if (sessionExpiry > currentTime)
                    {
                        await _logService.WarningAsync(
                            "Account",
                            "Login",
                            $"Login blocked. Active session already exists for UserName : {user.UserName}");

                        ModelState.AddModelError(
                            string.Empty,
                            "This user is already logged in from another browser or system.");

                        return View(model);
                    }

                    //==================================================
                    // Previous session expired
                    //==================================================

                    existingSession.IsActive = false;

                    await _context.SaveChangesAsync();
                }

                //======================================================
                // 3. Create New Session
                //======================================================

                Guid sessionToken = Guid.NewGuid();

                var activeSession = new UserActiveSession
                {
                    UserName = user.UserName,

                    SessionToken = sessionToken,

                    LoginDate = DateTimeHelper.Now,

                    LastActivityDate = DateTimeHelper.Now,

                    IsActive = true
                };

                _context.UserActiveSessions.Add(activeSession);

                await _context.SaveChangesAsync();

                //======================================================
                // 4. Create Claims
                //======================================================

                var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.UserID.ToString()),

            new Claim(
                ClaimTypes.Name,
                user.UserName),

            new Claim(
                "FullName",
                user.FullName ?? ""),

            new Claim(
                ClaimTypes.Email,
                user.Email ?? ""),

            new Claim(
                ClaimTypes.Role,
                user.Role ?? ""),

            new Claim(
                "Region",
                user.Region ?? ""),

            new Claim(
                "Module",
                user.Module ?? ""),

            new Claim(
                "DepartmentId",
                user.DepartmentId.ToString()),

            new Claim(
                "SessionToken",
                sessionToken.ToString())
        };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);


                //======================================================
                // 5. Sign In
                //======================================================

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        AllowRefresh = true
                    });

                //======================================================
                // 6. Existing Session Values
                //======================================================

                HttpContext.Session.SetInt32(
                    "DepartmentId",
                    user.DepartmentId);

                HttpContext.Session.SetString(
                    "DepartmentName",
                    DepartmentHelper.GetDepartmentName(
                        user.DepartmentId));

                HttpContext.Session.SetString(
                    "PortalTitle",
                    DepartmentHelper.GetPortalTitle(
                        user.DepartmentId));

                HttpContext.Session.SetString(
                    "DashboardTitle",
                    DepartmentHelper.GetDashboardTitle(
                        user.DepartmentId));

                await _logService.LoginAsync(
                    user.UserName,
                    DepartmentHelper.GetDepartmentName(
                        user.DepartmentId));


                //======================================================
                // 7. Return URL
                //======================================================

                if (!string.IsNullOrWhiteSpace(model.ReturnUrl) &&
                    Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }


                //======================================================
                // 8. Admin
                //======================================================

                if (string.Equals(
                    user.Role,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction(
                        "Index",
                        "Dashboard",
                        new { area = "Admin" });
                }

                //======================================================
                // 9. Engineer / Normal User
                //======================================================

                return RedirectToAction(
                    "Index",
                    "Dashboard");
            }
            catch (Exception ex)
            {
                await _logService.ErrorAsync(
                    "Account",
                    "Login",
                    ex);

                ModelState.AddModelError(
                    "",
                    "Unexpected error occurred.");

                return View(model);
            }
        }
        #endregion

        #region Logout

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var userName =
                User.Identity?.Name ?? "Unknown User";

            //======================================================
            // Get current session token
            //======================================================

            var sessionTokenValue =
                User.FindFirst("SessionToken")?.Value;

            if (Guid.TryParse(
                sessionTokenValue,
                out Guid sessionToken))
            {
                var activeSession =
                    await _context.UserActiveSessions
                        .FirstOrDefaultAsync(x =>
                            x.UserName == userName &&
                            x.SessionToken == sessionToken &&
                            x.IsActive);

                if (activeSession != null)
                {
                    activeSession.IsActive = false;

                    await _context.SaveChangesAsync();
                }
            }

            //======================================================
            // Existing logout log
            //======================================================

            await _logService.LogoutAsync(userName);

            //======================================================
            // Sign out
            //======================================================

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