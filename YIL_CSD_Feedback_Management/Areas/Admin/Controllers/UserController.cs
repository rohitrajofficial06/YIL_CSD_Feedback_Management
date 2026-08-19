using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels.User;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _service;
        private readonly IRegionService _regionService;
        private readonly ILogService _logService;

        public UserController(
     IUserService service,
     IRegionService regionService,
     ILogService logService)
        {
            _service = service;
            _regionService = regionService;
            _logService = logService;
        }

        private int GetDepartmentId()
        {
            var claim = User.FindFirst("DepartmentId")?.Value;

            if (!int.TryParse(claim, out int departmentId))
                departmentId = 1;

            return departmentId;
        }

        private string GetModule()
        {
            return User.FindFirst("Module")?.Value ?? "";
        }

        private async Task<List<SelectListItem>> GetRegionListAsync()
        {
            var regions = await _regionService.GetAllAsync();

            return regions
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new SelectListItem
                {
                    Value = x.RegionName,
                    Text = x.RegionName
                })
                .ToList();
        }

        public async Task<IActionResult> Index(
      string? searchText,
      string? region,
      string? role,
      bool? isActive,
      string? onlineStatus)
        {
            var departmentClaim = User.FindFirst("DepartmentId")?.Value;

            if (!int.TryParse(departmentClaim, out int departmentId))
            {
                departmentId = 1;
            }

            var model = await _service.GetUsersAsync(
                departmentId,
                searchText,
                region,
                role,
                isActive,
                onlineStatus);

            await _logService.InformationAsync(
                "User",
                "Index",
                "Viewed User List.");

            return View(model);
        }
        public async Task<IActionResult> Create()
        {
            var model = new UserCreateViewModel
            {
                RegionList = await GetRegionListAsync(),
                IsActive = true
            };

            await _logService.InformationAsync(
      "User",
      "Create(GET)",
      "Opened Create User page.");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await _logService.WarningAsync(
                    "User",
                    "Create",
                    "User creation failed due to validation errors.");

                model.RegionList = await GetRegionListAsync();
                return View(model);
            }

            await _service.CreateAsync(
                model,
                GetDepartmentId(),
                GetModule());

            await _logService.InformationAsync(
     "User",
     "Create",
     $"Created User : {model.UserName}, Role : {model.Role}, Region : {model.Region}");

            TempData["Success"] = "User created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var model = await _service.GetEditAsync(id, GetDepartmentId());

            if (model == null)
                return NotFound();

            model.RegionList = await GetRegionListAsync();

            await _logService.InformationAsync(
      "User",
      "Edit(GET)",
      $"Opened Edit Page. UserID : {id}");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await _logService.WarningAsync(
                    "User",
                    "Edit",
                    $"User update failed due to validation errors. UserID : {model.UserID}");

                model.RegionList = await GetRegionListAsync();
                return View(model);
            }

            await _service.UpdateAsync(
                model,
                GetDepartmentId());

            await _logService.InformationAsync(
      "User",
      "Edit",
      $"Updated User : {model.UserName} (UserID : {model.UserID})");

            TempData["Success"] = "User updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(long id)
        {
            var model = await _service.GetDetailsAsync(
                id,
                GetDepartmentId());

            if (model == null)
                return NotFound();

            await _logService.InformationAsync(
     "User",
     "Details",
     $"Viewed User Details. UserID : {id}");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(
                id,
                GetDepartmentId());

            await _logService.WarningAsync(
     "User",
     "Delete",
     $"Deleted User. UserID : {id}");

            TempData["Success"] = "User deleted successfully.";

            return RedirectToAction(nameof(Index));
        }


    }
}