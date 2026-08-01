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

        public UserController(
     IUserService service,
     IRegionService regionService)
        {
            _service = service;
            _regionService = regionService;
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

        public async Task<IActionResult> Index(string? searchText, string? region, string? role, bool? isActive)
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
                isActive);

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = new UserCreateViewModel
            {
                RegionList = await GetRegionListAsync(),
                IsActive = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RegionList = await GetRegionListAsync();
                return View(model);
            }

            await _service.CreateAsync(
                model,
                GetDepartmentId(),
                GetModule());

            TempData["Success"] = "User created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var model = await _service.GetEditAsync(id, GetDepartmentId());

            if (model == null)
                return NotFound();

            model.RegionList = await GetRegionListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RegionList = await GetRegionListAsync();
                return View(model);
            }

            await _service.UpdateAsync(
                model,
                GetDepartmentId());

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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(
                id,
                GetDepartmentId());

            TempData["Success"] = "User deleted successfully.";

            return RedirectToAction(nameof(Index));
        }


    }
}