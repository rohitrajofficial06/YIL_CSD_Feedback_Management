using System;

namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels.User
{
    public class UserListViewModel
    {
        public long UserID { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string Role { get; set; } = string.Empty;

        public string? Region { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsOnline { get; set; }

        public DateTime? LastActivityDate { get; set; }
    }
}