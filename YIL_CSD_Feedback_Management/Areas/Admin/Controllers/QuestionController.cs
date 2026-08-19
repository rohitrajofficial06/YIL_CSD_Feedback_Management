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
        private readonly ILogService _logService;

        public QuestionController(
     IQuestionService questionService,
     IDepartmentService departmentService,
     ILogService logService)
        {
            _questionService = questionService;
            _departmentService = departmentService;
            _logService = logService;
        }

        public async Task<IActionResult> Index(QuestionIndexViewModel model)
        {
            model = await _questionService.GetQuestionsAsync(model);

            await _logService.InformationAsync(
    "Question",
    "Index",
    "Viewed Question List.");

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

            await _logService.InformationAsync(
    "Question",
    "Create(GET)",
    "Opened Create Question page.");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await _logService.WarningAsync(
    "Question",
    "Create",
    "Question creation failed due to validation.");

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

            await _logService.InformationAsync(
    "Question",
    "Create",
    $"Created Question : {model.QuestionText}");

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

            await _logService.InformationAsync(
    "Question",
    "Edit(GET)",
    $"Opened Edit Question page. QuestionID : {id}");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(QuestionCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await _logService.WarningAsync(
    "Question",
    "Edit",
    $"Question update failed due to validation. QuestionID : {model.QuestionID}");

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

            await _logService.InformationAsync(
    "Question",
    "Edit",
    $"Updated Question : {model.QuestionText}");

            TempData["Success"] = "Question updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _questionService.DeleteAsync(id);

            await _logService.WarningAsync(
    "Question",
    "Delete",
    $"Deleted Question. QuestionID : {id}");

            TempData["Success"] = "Question deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}