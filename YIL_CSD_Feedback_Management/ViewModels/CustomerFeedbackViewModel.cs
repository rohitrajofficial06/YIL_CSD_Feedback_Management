using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using YIL_CSD_Feedback_Management.ViewModels.Feedback;

namespace YIL_CSD_Feedback_Management.ViewModels
{
    public class CustomerFeedbackViewModel
    {
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "First part is required.")]
        [RegularExpression(@"^\d{4}$",
     ErrorMessage = "Enter exactly 4 digits.")]
        public string? CaseNumberPart1 { get; set; }

        [Required(ErrorMessage = "Second part is required.")]
        [RegularExpression(@"^\d{5}$",
            ErrorMessage = "Enter exactly 5 digits.")]
        public string? CaseNumberPart2 { get; set; }

        public string? CaseNumber { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public string FeedbackSource { get; set; } = "Online";

        [Required]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        public string RespondentName { get; set; } = string.Empty;

        public string? Designation { get; set; }

        public string? ContactNo { get; set; }

        public string? EmailID { get; set; }

        public string? ServiceRequestNo { get; set; }

        public string? InstrumentCategory { get; set; }

        public string? YILEngineer { get; set; }

        public string? Comments { get; set; }

        public IFormFile? SignatureFile { get; set; }

        public List<QuestionViewModel> Questions { get; set; }
            = new();

        public List<SelectListItem> Departments { get; set; }
            = new();
    }
}