namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class MonthlyTrendViewModel
    {
        public string Month { get; set; } = "";

        public int ClosedCases { get; set; }

        public int FeedbackReceived { get; set; }

        public decimal CompletionPercentage { get; set; }

        public decimal AverageRating { get; set; }
    }
}