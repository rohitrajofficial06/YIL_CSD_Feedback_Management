namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class FeedbackListViewModel
    {
        public long FeedbackID { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string RespondentName { get; set; } = string.Empty;

        public string? YILEngineer { get; set; }
        public string? ServiceRequestNo { get; set; }

        public string? InstrumentCategory { get; set; }
        public string FeedbackSource { get; set; } = string.Empty;

        public decimal AverageRating { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsSubmitted { get; set; }

        public DateTime? ExcelClosedDate { get; set; }
        public string? FeedbackFileName { get; set; }
        public string? FeedbackFilePath { get; set; }
    }
}