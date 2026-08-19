namespace YIL_CSD_Feedback_Management.ViewModels.MyFeedback
{
    public class MyFeedbackDashboardViewModel
    {
        public int TotalFeedback { get; set; }

        public int OpenFeedback { get; set; }

        public int ClosedFeedback { get; set; }

        public DateTime? LastSubmittedDate { get; set; }

        public List<MyFeedbackListViewModel> Feedbacks { get; set; }
            = new();
    }
}