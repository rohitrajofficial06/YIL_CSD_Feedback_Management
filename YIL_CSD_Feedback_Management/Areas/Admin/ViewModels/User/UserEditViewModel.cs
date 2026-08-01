using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels.User
{
    public class UserEditViewModel
    {
        public long UserID { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public List<SelectListItem> RegionList { get; set; } = new();
    }
}