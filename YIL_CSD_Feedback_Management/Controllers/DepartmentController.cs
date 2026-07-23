using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YIL_CSD_Feedback_Management.Models;
using YIL_CSD_Feedback_Management.Services.Interfaces;
using YIL_CSD_Feedback_Management.ViewModels;

namespace YIL_CSD_Feedback_Management.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        #region Index

        public async Task<IActionResult> Index()
        {
            var data = await _departmentService.GetAllAsync();
            return View(data);
        }

        #endregion

        #region Create

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Department department = new Department
            {
                DepartmentCode = model.DepartmentCode.Trim(),
                DepartmentName = model.DepartmentName.Trim(),
                Description = model.Description,
                DisplayOrder = model.DisplayOrder
            };

            bool result = await _departmentService.SaveAsync(department);

            if (!result)
            {
                ModelState.AddModelError("", "Department Code already exists.");
                return View(model);
            }

            TempData["Success"] = "Department created successfully.";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Edit

        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);

            if (department == null)
                return NotFound();

            DepartmentViewModel model = new DepartmentViewModel
            {
                DepartmentID = department.DepartmentID,
                DepartmentCode = department.DepartmentCode,
                DepartmentName = department.DepartmentName,
                Description = department.Description,
                DisplayOrder = department.DisplayOrder
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DepartmentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var department = await _departmentService.GetByIdAsync(model.DepartmentID);

            if (department == null)
                return NotFound();

            department.DepartmentCode = model.DepartmentCode.Trim();
            department.DepartmentName = model.DepartmentName.Trim();
            department.Description = model.Description;
            department.DisplayOrder = model.DisplayOrder;
            department.ModifiedDate = DateTime.Now;
            department.ModifiedBy = "Admin";

            await _departmentService.UpdateAsync(department);

            TempData["Success"] = "Department updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Details

        public async Task<IActionResult> Details(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        #endregion

        #region Delete

        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _departmentService.DeleteAsync(id);

            TempData["Success"] = "Department deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}