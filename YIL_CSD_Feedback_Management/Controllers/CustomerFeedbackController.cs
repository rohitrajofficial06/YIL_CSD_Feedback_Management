using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using YIL_CSD_Feedback_Management.Services.Interfaces;
using YIL_CSD_Feedback_Management.ViewModels;
using YIL_CSD_Feedback_Management.ViewModels.Feedback;

namespace YIL_CSD_Feedback_Management.Controllers
{
    [Authorize]
    public class CustomerFeedbackController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IFeedbackQuestionService _questionService;
        private readonly ICustomerFeedbackService _feedbackService;
        private readonly IRegionService _regionService;

        public CustomerFeedbackController(
    IDepartmentService departmentService,
    IFeedbackQuestionService questionService,
    ICustomerFeedbackService feedbackService,
    IRegionService regionService)
        {
            _departmentService = departmentService;
            _questionService = questionService;
            _feedbackService = feedbackService;
            _regionService = regionService;
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

            switch (departmentId)
            {
                case 1:
                    model.IsCaseNumber = true;
                    model.IsReferenceNumber = false;
                    model.ReferenceLabel = "Case Number";
                    model.ReferenceType = "CASE";
                    break;

                case 2:
                    model.IsCaseNumber = false;
                    model.IsReferenceNumber = true;
                    model.ReferenceLabel = "Training Request Number (TRN)";
                    model.ReferenceType = "TRN";
                    break;

                case 3:
                    model.IsCaseNumber = false;
                    model.IsReferenceNumber = true;
                    model.ReferenceLabel = "Service Request Number (SRN)";
                    model.ReferenceType = "SRN";
                    break;

                default:
                    model.IsCaseNumber = true;
                    model.ReferenceLabel = "Case Number";
                    model.ReferenceType = "CASE";
                    break;
            }

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

            // Load Regions
            var regions = await _regionService.GetAllAsync();

            model.RegionList = regions.Select(r => new SelectListItem
            {
                Value = r.RegionName,
                Text = r.RegionName
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(CustomerFeedbackViewModel model)
        {

            if (model.IsCaseNumber)
            {
                if (string.IsNullOrWhiteSpace(model.CaseNumberPart1))
                {
                    ModelState.AddModelError(nameof(model.CaseNumberPart1),
                        "First part is required.");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(model.CaseNumberPart1, @"^\d{4}$"))
                {
                    ModelState.AddModelError(nameof(model.CaseNumberPart1),
                        "Enter exactly 4 digits.");
                }

                if (string.IsNullOrWhiteSpace(model.CaseNumberPart2))
                {
                    ModelState.AddModelError(nameof(model.CaseNumberPart2),
                        "Second part is required.");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(model.CaseNumberPart2, @"^\d{5}$"))
                {
                    ModelState.AddModelError(nameof(model.CaseNumberPart2),
                        "Enter exactly 5 digits.");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.ReferenceNumber))
                {
                    ModelState.AddModelError(nameof(model.ReferenceNumber),
                        $"{model.ReferenceLabel} is required.");
                }
            }

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

