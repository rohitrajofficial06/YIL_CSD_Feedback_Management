namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class ServiceFeedbackAnalyticsViewModel
    {
        public int? Month { get; set; }

        public int? Year { get; set; }

        public int TotalClosedCases { get; set; }

        public int TotalFeedbackReceived { get; set; }

        public decimal CompletionPercentage { get; set; }

        public decimal AverageRating { get; set; }

        public List<RegionAnalyticsViewModel> Regions { get; set; }
            = new();

        public List<MonthlyTrendViewModel> MonthlyTrend { get; set; }
    = new();
    }
}