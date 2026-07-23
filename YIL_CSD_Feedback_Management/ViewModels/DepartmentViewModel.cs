using System.ComponentModel.DataAnnotations;

namespace YIL_CSD_Feedback_Management.ViewModels
{
    public class DepartmentViewModel
    {
        public int DepartmentID { get; set; }

        [Required]
        [Display(Name = "Department Code")]
        public string DepartmentCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 1;
    }
}