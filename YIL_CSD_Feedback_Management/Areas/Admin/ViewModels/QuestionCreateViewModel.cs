using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class QuestionCreateViewModel
    {
        public int QuestionID { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentID { get; set; }

        [Required]
        [Display(Name = "Question No")]
        public int QuestionNo { get; set; }

        [Required]
        [Display(Name = "Question")]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Control Type")]
        public string ControlType { get; set; } = "Rating";

        public int? MinRating { get; set; } = 1;

        public int? MaxRating { get; set; } = 5;

        public bool IsMandatory { get; set; } = true;

        public int DisplayOrder { get; set; } = 1;

        public List<SelectListItem> Departments { get; set; }
            = new();
    }
}