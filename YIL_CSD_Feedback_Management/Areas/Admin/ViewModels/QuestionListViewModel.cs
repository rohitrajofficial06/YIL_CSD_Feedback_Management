namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class QuestionListViewModel
    {
        public int QuestionID { get; set; }

        public int DepartmentID { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public int QuestionNo { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public string ControlType { get; set; } = string.Empty;

        public int? MaxRating { get; set; }

        public int? MinRating { get; set; }

        public bool IsMandatory { get; set; }

        public int DisplayOrder { get; set; }
    }
}