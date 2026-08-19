namespace YIL_CSD_Feedback_Management.ViewModels.MyFeedback
{
    public class MyFeedbackListViewModel
    {
        public long FeedbackID { get; set; }

        public string CaseNumber { get; set; } = "";

        public string CompanyName { get; set; } = "";

        public string RespondentName { get; set; } = "";

        public string Region { get; set; } = "";

        public string FeedbackStatus { get; set; } = "";

        public double AverageRating { get; set; }

        public DateTime? CreatedDate { get; set; }

        public bool CanEdit { get; set; }
    }
}