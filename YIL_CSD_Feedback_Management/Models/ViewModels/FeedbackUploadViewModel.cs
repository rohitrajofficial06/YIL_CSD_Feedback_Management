using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace YIL_CSD_Feedback_Management.Models.ViewModels
{
    public class FeedbackUploadViewModel
    {
        [Display(Name = "Department")]
        public int? DepartmentID { get; set; }

        public List<SelectListItem> Departments { get; set; } = new();

        [Required(ErrorMessage = "Please select a feedback file.")]
        public IFormFile? FeedbackFile { get; set; }

        public IEnumerable<FeedbackUpload> UploadedFiles { get; set; } = new List<FeedbackUpload>();
    }
}