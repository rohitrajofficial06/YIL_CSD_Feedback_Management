namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class CriticalFeedbackListViewModel
    {
        public long FeedbackID { get; set; }

        public string CompanyName { get; set; }

        public string RespondentName { get; set; }

        public string YILEngineer { get; set; }

        public string ServiceRequestNo { get; set; }

        public string Region { get; set; }

        public string FeedbackSource { get; set; }

        public decimal AverageRating { get; set; }

        public decimal LowestRating { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime? ExcelClosedDate { get; set; }
    }
}