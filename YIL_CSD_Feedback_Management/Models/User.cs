using System.ComponentModel.DataAnnotations;

namespace YIL_CSD_Feedback_Management.Models
{
    public class User
    {
        public long UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string Role { get; set; } = string.Empty;

        [StringLength(50)]
        public string Region { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string? Module { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime CreatedDate { get; set; }

        public int DepartmentId { get; set; }
    }
}