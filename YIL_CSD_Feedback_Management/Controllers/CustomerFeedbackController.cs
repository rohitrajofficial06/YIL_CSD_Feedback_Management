using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using YIL_CSD_Feedback_Management.Helpers;
using YIL_CSD_Feedback_Management.Services.Interfaces;
using YIL_CSD_Feedback_Management.ViewModels;
using YIL_CSD_Feedback_Management.ViewModels.Feedback;
using YIL_CSD_Feedback_Management.ViewModels.MyFeedback;

namespace YIL_CSD_Feedback_Management.Controllers
{
    [Authorize]
    public class CustomerFeedbackController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IFeedbackQuestionService _questionService;
        private readonly ICustomerFeedbackService _feedbackService;
        private readonly IRegionService _regionService;
        private readonly IMyFeedbackService _myFeedbackService;
        private readonly ILogService _logService;

        public CustomerFeedbackController(
     IDepartmentService departmentService,
     IFeedbackQuestionService questionService,
     ICustomerFeedbackService feedbackService,
     IRegionService regionService,
     IMyFeedbackService myFeedbackService,
     ILogService logService)
        {
            _departmentService = departmentService;
            _questionService = questionService;
            _feedbackService = feedbackService;
            _regionService = regionService;
            _myFeedbackService = myFeedbackService;
            _logService = logService;
        }

        // ===========================================
        // Upload or Fill
        // ===========================================

        public async Task<IActionResult> SelectFeedbackType()
        {
            int departmentId = Convert.ToInt32(
                User.FindFirst("DepartmentId")?.Value);

            CustomerFeedbackViewModel model = new();

            model.DepartmentID = departmentId;

            await _logService.InformationAsync(
    "Customer Feedback",
    "SelectFeedbackType",
    $"Opened Feedback Type Selection. DepartmentID : {departmentId}");

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

            model.YILEngineer =
                            User.FindFirst("FullName")?.Value
                            ?? User.Identity?.Name
                            ?? "";

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

            await _logService.InformationAsync(
    "Customer Feedback",
    "FillFeedback",
    $"Opened Feedback Form. DepartmentID : {departmentId}");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(CustomerFeedbackViewModel model)
        {
            //==========================================================
            // Rebuild Department Settings
            //==========================================================

            switch (model.DepartmentID)
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
                    model.IsReferenceNumber = false;
                    model.ReferenceLabel = "Case Number";
                    model.ReferenceType = "CASE";
                    break;
            }

           
            //==========================================================
            // Validate Case Number / Reference Number
            //==========================================================

            if (model.IsCaseNumber)
            {
                if (string.IsNullOrWhiteSpace(model.CaseNumberPart1))
                {
                    ModelState.AddModelError(
                        nameof(model.CaseNumberPart1),
                        "First part is required.");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(
                    model.CaseNumberPart1,
                    @"^\d{4}$"))
                {
                    ModelState.AddModelError(
                        nameof(model.CaseNumberPart1),
                        "Enter exactly 4 digits.");
                }

                if (string.IsNullOrWhiteSpace(model.CaseNumberPart2))
                {
                    ModelState.AddModelError(
                        nameof(model.CaseNumberPart2),
                        "Second part is required.");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(
                    model.CaseNumberPart2,
                    @"^\d{5}$"))
                {
                    ModelState.AddModelError(
                        nameof(model.CaseNumberPart2),
                        "Enter exactly 5 digits.");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.ReferenceNumber))
                {
                    ModelState.AddModelError(
                        nameof(model.ReferenceNumber),
                        $"{model.ReferenceLabel} is required.");
                }

                // Future Enhancement:
                // Validate duplicate TRN / SRN here.
            }

            //==========================================================
            // Rating Validation (ALL Departments)
            //==========================================================

            for (int i = 0; i < model.Questions.Count; i++)
            {
                if (!model.Questions[i].RatingValue.HasValue &&
                    !model.Questions[i].IsNotApplicable)
                {
                    ModelState.AddModelError(
                        $"Questions[{i}].RatingValue",
                        $"Please select a rating or mark N/A for Question {i + 1}.");
                }
            }

            //------------------------------------------------------
            // Duplicate Case Number / TRN / SRN Validation
            //------------------------------------------------------

            if (ModelState.IsValid)
            {
                string referenceNumber;

                if (model.IsCaseNumber)
                {
                    referenceNumber =
                        $"YIL-C{model.CaseNumberPart1}-{model.CaseNumberPart2}";
                }
                else
                {
                    referenceNumber = model.ReferenceNumber!;
                }

                bool exists = await _feedbackService
                    .CaseNumberExistsAsync(referenceNumber);

                if (exists)
                {
                    if (model.IsCaseNumber)
                    {
                        ModelState.AddModelError(
                            nameof(model.CaseNumberPart2),
                            "Case Number already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError(
                            nameof(model.ReferenceNumber),
                            $"{model.ReferenceLabel} already exists.");
                    }
                }
            }

            //==========================================================
            // Feedback Attachment Validation
            //==========================================================

            if (!model.IsEditMode &&
                (model.FeedbackFile == null || model.FeedbackFile.Length == 0))
            {
                ModelState.AddModelError(
                    nameof(model.FeedbackFile),
                    "Please attach the feedback document.");
            }


            //==========================================================
            // Validation Failed
            //==========================================================


            if (!ModelState.IsValid)
            {
                //---------------------------------------
                // Reset Rating for N/A
                //---------------------------------------

                foreach (var question in model.Questions)
                {
                    if (question.IsNotApplicable)
                    {
                        question.RatingValue = null;
                    }
                }

                //---------------------------------------
                // Rebuild Department Settings
                //---------------------------------------

                switch (model.DepartmentID)
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
                        model.IsReferenceNumber = false;
                        model.ReferenceLabel = "Case Number";
                        model.ReferenceType = "CASE";
                        break;
                }

                //---------------------------------------
                // Reload Department Name
                //---------------------------------------

                var departments = await _departmentService.GetAllAsync();

                var department = departments.FirstOrDefault(
                    x => x.DepartmentID == model.DepartmentID);

                if (department != null)
                {
                    model.DepartmentName = department.DepartmentName;
                }

                //---------------------------------------
                // Reload Questions
                //---------------------------------------

                var questions = await _questionService
                    .GetByDepartmentAsync(model.DepartmentID);

                foreach (var dbQuestion in questions)
                {
                    var existing = model.Questions
                        .FirstOrDefault(x => x.QuestionID == dbQuestion.QuestionID);

                    if (existing != null)
                    {
                        existing.QuestionNo = dbQuestion.QuestionNo;
                        existing.QuestionText = dbQuestion.QuestionText;
                        existing.ControlType = dbQuestion.ControlType;
                        existing.IsMandatory = dbQuestion.IsMandatory;
                    }
                }

                //---------------------------------------
                // Reload Region List
                //---------------------------------------

                var regions = await _regionService.GetAllAsync();

                model.RegionList = regions
                    .Select(r => new SelectListItem
                    {
                        Value = r.RegionName,
                        Text = r.RegionName
                    })
                    .ToList();

                //---------------------------------------
                // Log
                //---------------------------------------

                await _logService.WarningAsync(
                    "Customer Feedback",
                    "SubmitFeedback",
                    "Validation failed.");

                //---------------------------------------
                // Return Same Page
                //---------------------------------------

                return View("FillFeedback", model);
            }

            //==========================================================
            // Save Feedback
            //==========================================================

            long feedbackId = await _feedbackService.SaveAsync(
                model,
                User.Identity?.Name ?? "Unknown");

            await _logService.InformationAsync(
                "Customer Feedback",
                "SubmitFeedback",
                $"Feedback Submitted Successfully. FeedbackID : {feedbackId}");

            return RedirectToAction(nameof(ThankYou));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFeedback(CustomerFeedbackViewModel model)
        {
            string createdBy = User.Identity?.Name ?? "";

            var feedback = await _feedbackService.GetForEditAsync(
                model.FeedbackID,
                createdBy);

            if (feedback == null)
            {
                TempData["Error"] =
                    "This feedback can no longer be edited.";

                return RedirectToAction(nameof(MyFeedback));
            }

            // Update editable fields only

            feedback.CompanyName = model.CompanyName;

            feedback.RespondentName = model.RespondentName;

            feedback.Designation = model.Designation;

            feedback.ContactNo = model.ContactNo;

            feedback.EmailID = model.EmailID;

            feedback.ServiceRequestNo = model.ServiceRequestNo;

            feedback.InstrumentCategory = model.InstrumentCategory;

            feedback.YILEngineer = model.YILEngineer;

            feedback.Region = model.Region;

            feedback.Comments = model.Comments;

            feedback.ModifiedBy = createdBy;

            feedback.ModifiedDate = DateTimeHelper.Now;

            //------------------------------------------------------
            // Update Ratings
            //------------------------------------------------------

            foreach (var question in model.Questions)
            {
                var rating = feedback.Ratings
                    .FirstOrDefault(x => x.QuestionNo == question.QuestionNo);

                if (rating == null)
                    continue;

                rating.RatingValue = question.RatingValue;

                rating.IsNotApplicable = question.IsNotApplicable;
            }

            await _feedbackService.UpdateAsync(feedback);

            await _logService.InformationAsync(
                "Customer Feedback",
                "UpdateFeedback",
                $"Feedback Updated Successfully. FeedbackID : {feedback.FeedbackID}");

            TempData["Success"] =
                "Feedback updated successfully.";

            return RedirectToAction(nameof(Details),
                          new { id = feedback.FeedbackID });
        }
        // ===========================================
        // Site Service Form
        // ===========================================

        public async Task<IActionResult> SiteService()
        {
            await _logService.InformationAsync(
    "Customer Feedback",
    "SiteService",
    "Opened Site Service Feedback.");

            return View();
        }

        // ===========================================
        // Training
        // ===========================================

        public async Task<IActionResult> Training()
        {
            await _logService.InformationAsync(
    "Customer Feedback",
    "Training",
    "Opened Training Feedback.");

            return View();
        }

        // ===========================================
        // Bench Repair
        // ===========================================

        public async Task<IActionResult> BenchRepair()
        {
            await _logService.InformationAsync(
    "Customer Feedback",
    "BenchRepair",
    "Opened Bench Repair Feedback.");

            return View();
        }

        // ===========================================
        // Thank You
        // ===========================================

        public async Task<IActionResult> MyFeedback(
      string? searchText,
      string? status,
      string? region,
      DateTime? fromDate,
      DateTime? toDate,
      int pageNumber = 1,
      int pageSize = 10,
      string sortColumn = "CreatedDate",
      string sortOrder = "desc")
        {
            string createdBy = User.Identity?.Name ?? "";

            var model = await _myFeedbackService.GetMyFeedbackAsync(
                createdBy,
                searchText,
                status,
                region,
                fromDate,
                toDate,
                pageNumber,
                pageSize,
                sortColumn,
                sortOrder);

            await _logService.InformationAsync(
                "Customer Feedback",
                "MyFeedback",
                $"{createdBy} viewed My Feedback.");

            return View(model);
        }

        public async Task<IActionResult> Details(long id)
        {
            string createdBy = User.Identity?.Name ?? "";

            var model = await _myFeedbackService.GetDetailsAsync(
                id,
                createdBy);

            if (model == null)
            {
                return NotFound();
            }

            await _logService.InformationAsync(
                "Customer Feedback",
                "Details",
                $"{createdBy} viewed FeedbackID : {id}");

            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> ValidateCaseNumber(
      string? part1,
      string? part2,
      string? referenceNumber)
        {
            string number = "";

            if (!string.IsNullOrWhiteSpace(referenceNumber))
            {
                number = referenceNumber.Trim();
            }
            else
            {
                number = $"YIL-C{part1}-{part2}";
            }

            bool exists = await _feedbackService
                .CaseNumberExistsAsync(number);

            return Json(new
            {
                valid = !exists
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            string createdBy = User.Identity?.Name ?? "";

            var feedback = await _feedbackService.GetForEditAsync(
                id,
                createdBy);

            if (feedback == null)
            {
                TempData["Error"] =
                    "This feedback cannot be edited. Either it has expired or has already been closed.";

                return RedirectToAction(nameof(MyFeedback));
            }

            CustomerFeedbackViewModel model = new();

            model.IsEditMode = true;

            model.FeedbackID = feedback.FeedbackID;

            model.DepartmentID = feedback.DepartmentID;

            model.CompanyName = feedback.CompanyName;

            model.RespondentName = feedback.RespondentName;

            model.Designation = feedback.Designation;

            model.ContactNo = feedback.ContactNo;

            model.EmailID = feedback.EmailID;

            model.ServiceRequestNo = feedback.ServiceRequestNo;

            model.InstrumentCategory = feedback.InstrumentCategory;

            model.YILEngineer = feedback.YILEngineer;

            model.Region = feedback.Region;

            model.Comments = feedback.Comments;

            model.ReferenceType = feedback.ReferenceType;

            switch (feedback.DepartmentID)
            {
                case 1:

                    model.IsCaseNumber = true;

                    model.ReferenceLabel = "Case Number";

                    if (!string.IsNullOrWhiteSpace(feedback.CaseNumber))
                    {
                        string number =
                            feedback.CaseNumber.Replace("YIL-C", "");

                        string[] parts = number.Split('-');

                        if (parts.Length == 2)
                        {
                            model.CaseNumberPart1 = parts[0];

                            model.CaseNumberPart2 = parts[1];
                        }
                    }

                    break;

                case 2:

                    model.IsReferenceNumber = true;

                    model.ReferenceLabel = "Training Request Number (TRN)";

                    model.ReferenceNumber = feedback.CaseNumber;

                    break;

                case 3:

                    model.IsReferenceNumber = true;

                    model.ReferenceLabel = "Service Request Number (SRN)";

                    model.ReferenceNumber = feedback.CaseNumber;

                    break;
            }
            var regions = await _regionService.GetAllAsync();

           
            model.RegionList = regions
                .Select(r => new SelectListItem
                {
                    Value = r.RegionName,
                    Text = r.RegionName
                })
                .ToList();

            var questions =
    await _questionService.GetByDepartmentAsync(
        feedback.DepartmentID);

            model.Questions = questions
                .Select(q =>
                {
                    var rating = feedback.Ratings
                        .FirstOrDefault(r =>
                            r.QuestionNo == q.QuestionNo);

                    return new QuestionViewModel
                    {
                        QuestionID = q.QuestionID,

                        QuestionNo = q.QuestionNo,

                        QuestionText = q.QuestionText,

                        ControlType = q.ControlType,

                        IsMandatory = q.IsMandatory,

                        RatingValue = rating?.RatingValue,

                        IsNotApplicable =
                            rating?.IsNotApplicable ?? false
                    };
                })
                .ToList();

            await _logService.InformationAsync(
    "Customer Feedback",
    "Edit",
    $"Opened Edit Form. FeedbackID : {id}");

            return View("FillFeedback", model);
        }

        public async Task<IActionResult> ThankYou()
        {
            await _logService.InformationAsync(
    "Customer Feedback",
    "ThankYou",
    "Feedback Submitted Successfully.");

            return View();
        }
    }
}

