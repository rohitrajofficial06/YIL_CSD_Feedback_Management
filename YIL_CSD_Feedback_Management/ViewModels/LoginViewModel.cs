using System.ComponentModel.DataAnnotations;

namespace YIL_CSD_Feedback_Management.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your user name.")]
        [Display(Name = "User Name")]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        // Used to display login errors returned by the controller
        public string? ErrorMessage { get; set; }

        // Optional: Return URL after successful login
        public string? ReturnUrl { get; set; }
    }
}