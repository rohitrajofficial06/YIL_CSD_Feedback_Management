namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class CriticalFeedbackIndexViewModel
    {
        public string SearchText { get; set; }

        public string Region { get; set; }

        public string FeedbackSource { get; set; }

        public int? Rating { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public string SortColumn { get; set; }

        public string SortDirection { get; set; }

        public int TotalCriticalFeedback { get; set; }

        public decimal AverageRating { get; set; }

        public int OnlineFeedback { get; set; }

        public int UploadedFeedback { get; set; }

        public List<CriticalFeedbackListViewModel> Feedbacks { get; set; }
            = new();
    }
}