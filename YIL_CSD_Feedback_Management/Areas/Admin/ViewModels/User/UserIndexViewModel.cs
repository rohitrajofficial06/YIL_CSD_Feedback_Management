using System.Collections.Generic;

namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels.User
{
    public class UserIndexViewModel
    {
        public List<UserListViewModel> Users { get; set; } = new();

        public string? SearchText { get; set; }

        public string? Region { get; set; }

        public string? Role { get; set; }

        public bool? IsActive { get; set; }

        // Online / Offline filter
        public string? OnlineStatus { get; set; }

        // Summary
        public int TotalUsers { get; set; }

        public int OnlineUsers { get; set; }

        public int OfflineUsers { get; set; }
    }
}