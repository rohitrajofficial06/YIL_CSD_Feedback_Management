namespace YIL_CSD_Feedback_Management.ViewModels.Dashboard
{
    public class EngineerDashboardViewModel
    {
        // Logged-in User
        public string FullName { get; set; } = "";

        public string Region { get; set; } = "";

        public string Module { get; set; } = "";

        public int DepartmentId { get; set; }

        // Dashboard Cards
        public int TotalFeedback { get; set; }

        public int OpenFeedback { get; set; }

        public int ClosedFeedback { get; set; }

        public double AverageRating { get; set; }

        // Greeting
        public string Greeting { get; set; } = "";

        // Recent Feedback
        public List<RecentFeedbackViewModel> RecentFeedbacks { get; set; }
            = new();
    }

    public class RecentFeedbackViewModel
    {
        public long FeedbackID { get; set; }

        public string CaseNumber { get; set; } = "";

        public string CompanyName { get; set; } = "";

        public string FeedbackStatus { get; set; } = "";

        public DateTime? CreatedDate { get; set; }
    }
}