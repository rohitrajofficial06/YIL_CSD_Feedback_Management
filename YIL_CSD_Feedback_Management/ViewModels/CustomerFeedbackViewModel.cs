using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using YIL_CSD_Feedback_Management.ViewModels.Feedback;

namespace YIL_CSD_Feedback_Management.ViewModels
{
    public class CustomerFeedbackViewModel
    {
        public long FeedbackID { get; set; }
        public bool IsEditMode { get; set; }

        public bool CanEdit { get; set; }
        public int DepartmentID { get; set; }

        public string? CaseNumberPart1 { get; set; }

        public string? CaseNumberPart2 { get; set; }

        public string? CaseNumber { get; set; }

        public string? ReferenceNumber { get; set; }

        public string ReferenceLabel { get; set; } = "";

        public string ReferenceType { get; set; } = "";

        public bool IsCaseNumber { get; set; }

        public bool IsReferenceNumber { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public string FeedbackSource { get; set; } = "Online";

        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "Please enter Company Name.")]
        public string CompanyName { get; set; } = string.Empty;

        [Display(Name = "Respondent Name")]
        [Required(ErrorMessage = "Please enter Respondent Name.")]
        public string RespondentName { get; set; } = string.Empty;

        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Please enter Designation.")]
        public string? Designation { get; set; }

        [Display(Name = "Contact No")]
        public string? ContactNo { get; set; }

        [Display(Name = "Email ID")]
        public string? EmailID { get; set; }

        [Display(Name = "Service Request No")]
        [Required(ErrorMessage = "Please enter Service Request Number.")]
        public string? ServiceRequestNo { get; set; }

        [Display(Name = "Instrument Category")]
        public string? InstrumentCategory { get; set; }

        [Display(Name = "YIL Engineer")]
        public string? YILEngineer { get; set; }
      
        [Required(ErrorMessage = "Please select Region.")]
        [Display(Name = "Region")]
        public string? Region { get; set; }

        public string? Comments { get; set; }

        [Required(ErrorMessage = "Please attach the feedback document.")]
        public IFormFile? FeedbackFile { get; set; }

        public List<SelectListItem> RegionList { get; set; } = new();

        public IFormFile? SignatureFile { get; set; }

        public List<QuestionViewModel> Questions { get; set; }
            = new();

        public List<SelectListItem> Departments { get; set; }
            = new();
    }
}