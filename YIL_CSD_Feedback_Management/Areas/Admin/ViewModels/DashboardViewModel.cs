namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
        // ============================
        // KPI Cards
        // ============================

        public int TotalFeedback { get; set; }

        public decimal AverageRating { get; set; }

        public int TodayFeedback { get; set; }

        public decimal CustomerSatisfaction { get; set; }

        public int OnlineFeedback { get; set; }

        public int UploadedFeedback { get; set; }

        public int TotalQuestions { get; set; }

        // ============================
        // Recent Feedback
        // ============================

        public List<RecentFeedbackViewModel> RecentFeedbacks { get; set; }
            = new();
    }

    public class RecentFeedbackViewModel
    {
        public long FeedbackID { get; set; }

        public string CaseNumber { get; set; } = string.Empty;

        public string FeedbackStatus { get; set; } = "Open";

        public string CompanyName { get; set; } = string.Empty;

        public string RespondentName { get; set; } = string.Empty;

        public string EngineerName { get; set; } = string.Empty;

        public string FeedbackSource { get; set; } = string.Empty;

        public decimal Rating { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}