namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class RegionDetailsViewModel
    {
        public string CaseNumber { get; set; } = string.Empty;

        public string? YILEngineer { get; set; }

        public DateTime? ClosedDate { get; set; }

        public bool FeedbackReceived { get; set; }

        public decimal? Rating { get; set; }
    }
}