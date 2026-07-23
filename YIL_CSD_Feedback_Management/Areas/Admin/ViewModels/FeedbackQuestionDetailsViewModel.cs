namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class FeedbackQuestionDetailsViewModel
    {
        public int QuestionNo { get; set; }

        public string QuestionText { get; set; } = "";

        public int? Rating { get; set; }

        public bool IsNotApplicable { get; set; }
    }
}