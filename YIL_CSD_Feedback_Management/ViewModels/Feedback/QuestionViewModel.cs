namespace YIL_CSD_Feedback_Management.ViewModels.Feedback
{
    public class QuestionViewModel
    {
        public int QuestionID { get; set; }

        public int QuestionNo { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public string ControlType { get; set; } = "Rating";

        public bool IsMandatory { get; set; }

        // Customer Answer
        public int? RatingValue { get; set; }

        public string? TextAnswer { get; set; }

        public bool IsNotApplicable { get; set; }
    }
}