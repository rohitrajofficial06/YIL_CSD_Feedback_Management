namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class ServiceFeedbackAnalyticsViewModel
    {
        public int? Month { get; set; }

        public int? Year { get; set; }

        public string MonthName { get; set; } = string.Empty;

        public int TotalClosedCases { get; set; }

        public int TotalFeedbackReceived { get; set; }

        public int TotalPendingFeedback { get; set; }

        public decimal CompletionPercentage { get; set; }

        public decimal AverageRating { get; set; }

        public List<RegionAnalyticsViewModel> Regions { get; set; }
            = new();

        public List<MonthlyTrendViewModel> MonthlyTrend { get; set; }
            = new();
    }
}