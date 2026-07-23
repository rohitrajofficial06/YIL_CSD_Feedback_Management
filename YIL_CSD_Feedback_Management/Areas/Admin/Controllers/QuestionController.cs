using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.ViewModels;
using YIL_CSD_Feedback_Management.Services.Interfaces;

namespace YIL_CSD_Feedback_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly IDepartmentService _departmentService;

        public QuestionController(
     IQuestionService questionService,
     IDepartmentService departmentService)
        {
            _questionService = questionService;

            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index(QuestionIndexViewModel model)
        {
            model = await _questionService.GetQuestionsAsync(model);

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            QuestionCreateViewModel model = new();

            var departments = await _departmentService.GetAllAsync();

            model.Departments = departments
                .Select(x => new SelectListItem
                {
                    Value = x.DepartmentID.ToString(),
                    Text = x.DepartmentName
                })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.GetAllAsync();

                model.Departments = departments
                    .Select(x => new SelectListItem
                    {
                        Value = x.DepartmentID.ToString(),
                        Text = x.DepartmentName
                    })
                    .ToList();

                return View(model);
            }

            await _questionService.CreateAsync(model);

            TempData["Success"] = "Question created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _questionService.GetByIdAsync(id);

            if (model == null)
                return NotFound();

            var departments = await _departmentService.GetAllAsync();

            model.Departments = departments
                .Select(x => new SelectListItem
                {
                    Value = x.DepartmentID.ToString(),
                    Text = x.DepartmentName
                })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(QuestionCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.GetAllAsync();

                model.Departments = departments
                    .Select(x => new SelectListItem
                    {
                        Value = x.DepartmentID.ToString(),
                        Text = x.DepartmentName
                    })
                    .ToList();

                return View(model);
            }

            await _questionService.UpdateAsync(model);

            TempData["Success"] = "Question updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _questionService.DeleteAsync(id);

            TempData["Success"] = "Question deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}