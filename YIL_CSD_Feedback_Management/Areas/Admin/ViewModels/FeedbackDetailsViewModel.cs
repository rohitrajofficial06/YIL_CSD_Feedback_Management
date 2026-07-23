using System;
using System.Collections.Generic;

namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class FeedbackDetailsViewModel
    {
        public long FeedbackID { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string RespondentName { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;

        public string ContactNo { get; set; } = string.Empty;

        public string EmailID { get; set; } = string.Empty;

        public string ServiceRequestNo { get; set; } = string.Empty;

        public string InstrumentCategory { get; set; } = string.Empty;

        public string YILEngineer { get; set; } = string.Empty;

        public string DepartmentName { get; set; } = string.Empty;

        public string Comments { get; set; } = string.Empty;

        public string FeedbackSource { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public List<FeedbackQuestionDetailsViewModel> Questions { get; set; }
            = new List<FeedbackQuestionDetailsViewModel>();
    }
}