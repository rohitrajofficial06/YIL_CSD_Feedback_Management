using Microsoft.AspNetCore.Mvc.Rendering;

namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class FeedbackIndexViewModel
    {
        //============================
        // Summary
        //============================

        public int TotalFeedback { get; set; }

        public int OnlineFeedback { get; set; }

        public int UploadedFeedback { get; set; }

        public decimal AverageRating { get; set; }

        //============================
        // Search
        //============================

        public string? SearchText { get; set; }

        public string? FeedbackSource { get; set; }

        public int? Rating { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        // Future Department Dropdown
        public int? DepartmentId { get; set; }

        public List<SelectListItem> Departments { get; set; }
            = new();

        //============================
        // Paging
        //============================

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string SortColumn { get; set; } = "CreatedDate";

        public string SortDirection { get; set; } = "desc";

        public int TotalRecords { get; set; }
        //public int? FromMonth { get; set; }

        //public int? ToMonth { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalRecords / PageSize);

        //============================
        // Grid
        //============================

        public List<FeedbackListViewModel> Feedbacks { get; set; }
            = new();
    }
}