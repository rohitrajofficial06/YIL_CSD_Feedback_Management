using YIL_CSD_Feedback_Management.ViewModels.Feedback;
using YIL_CSD_Feedback_Management.Services.Interfaces;
using YIL_CSD_Feedback_Management.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YIL_CSD_Feedback_Management.Controllers
{
    [Authorize]
    public class CustomerFeedbackController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IFeedbackQuestionService _questionService;
        private readonly ICustomerFeedbackService _feedbackService;

        public CustomerFeedbackController(
     IDepartmentService departmentService,
     IFeedbackQuestionService questionService,
     ICustomerFeedbackService feedbackService)
        {
            _departmentService = departmentService;

            _questionService = questionService;

            _feedbackService = feedbackService;
        }


        // ===========================================
        // Upload or Fill
        // ===========================================

        public IActionResult SelectFeedbackType()
        {
            int departmentId = Convert.ToInt32(
                User.FindFirst("DepartmentId")?.Value);

            CustomerFeedbackViewModel model = new();

            model.DepartmentID = departmentId;

            return View(model);
        }

        // ===========================================
        // Dynamic Form
        // ===========================================

        public async Task<IActionResult> FillFeedback()
        {
            var departmentClaim = User.FindFirst("DepartmentId");

            if (departmentClaim == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            int departmentId = Convert.ToInt32(departmentClaim.Value);

            CustomerFeedbackViewModel model = new();

            model.DepartmentID = departmentId;

            var departments = await _departmentService.GetAllAsync();

            var department = departments
                .FirstOrDefault(x => x.DepartmentID == departmentId);

            if (department != null)
            {
                model.DepartmentName = department.DepartmentName;
            }

            var questions = await _questionService.GetByDepartmentAsync(departmentId);

            model.Questions = questions.Select(x => new QuestionViewModel
            {
                QuestionID = x.QuestionID,
                QuestionNo = x.QuestionNo,
                QuestionText = x.QuestionText,
                ControlType = x.ControlType,
                IsMandatory = x.IsMandatory
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(CustomerFeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                    });

                return Json(errors);
            }

            long feedbackId = await _feedbackService.SaveAsync(model);

            return RedirectToAction(nameof(ThankYou));
        }

        // ===========================================
        // Site Service Form
        // ===========================================

        public IActionResult SiteService()
        {
            return View();
        }

        // ===========================================
        // Training
        // ===========================================

        public IActionResult Training()
        {
            return View();
        }

        // ===========================================
        // Bench Repair
        // ===========================================

        public IActionResult BenchRepair()
        {
            return View();
        }

        // ===========================================
        // Thank You
        // ===========================================

        public IActionResult ThankYou()
        {
            return View();
        }
    }
}

