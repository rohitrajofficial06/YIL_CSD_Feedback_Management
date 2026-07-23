using System.Collections.Generic;

namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class FeedbackDashboardViewModel
    {
        public int TotalFeedback { get; set; }

        public int OnlineFeedback { get; set; }

        public int UploadedFeedback { get; set; }

        public decimal AverageRating { get; set; }

        public List<FeedbackListViewModel> Feedbacks { get; set; }
            = new List<FeedbackListViewModel>();
    }
}