namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class MonthlyTrendViewModel
    {
        public string MonthName { get; set; } = string.Empty;

        public int ClosedCases { get; set; }

        public int FeedbackReceived { get; set; }

        public decimal Percentage { get; set; }

        public decimal AverageRating { get; set; }
    }
}