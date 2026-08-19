using System.ComponentModel.DataAnnotations;

namespace YIL_CSD_Feedback_Management.ViewModels.MyFeedback
{
    public class MyFeedbackDetailsViewModel
    {
        public int DepartmentID { get; set; }
        public long FeedbackID { get; set; }

        public string CaseNumber { get; set; } = "";

        public string CompanyName { get; set; } = "";

        public string RespondentName { get; set; } = "";

        public string Designation { get; set; } = "";

        public string ContactNo { get; set; } = "";

        public string EmailID { get; set; } = "";

        public string ServiceRequestNo { get; set; } = "";

        public string InstrumentCategory { get; set; } = "";

        public string YILEngineer { get; set; } = "";

        public string Region { get; set; } = "";

        public string Comments { get; set; } = "";

        public bool CanEdit { get; set; }
        public string FeedbackStatus { get; set; } = "";

        public string? SignaturePath { get; set; }

        public DateTime? CreatedDate { get; set; }

        public List<MyFeedbackQuestionViewModel> Questions { get; set; }
            = new();
    }

    public class MyFeedbackQuestionViewModel
    {
        public int QuestionNo { get; set; }

        public string QuestionText { get; set; } = "";

        public int? RatingValue { get; set; }

        public bool IsNotApplicable { get; set; }
    }
}