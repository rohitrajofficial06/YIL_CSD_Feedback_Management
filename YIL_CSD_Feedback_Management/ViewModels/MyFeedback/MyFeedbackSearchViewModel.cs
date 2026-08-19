using Microsoft.AspNetCore.Mvc.Rendering;

namespace YIL_CSD_Feedback_Management.ViewModels.MyFeedback
{
    public class MyFeedbackSearchViewModel
    {
        public string? SearchText { get; set; }

        public string? Status { get; set; }

        public string? Region { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public List<SelectListItem> StatusList { get; set; } = new();

        public List<SelectListItem> RegionList { get; set; } = new();

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalRecords { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalRecords / PageSize);

        //========================================
        // Feedback List
        //========================================

        public List<MyFeedbackListViewModel> Feedbacks { get; set; }
            = new();

        public string SortColumn { get; set; } = "CreatedDate";

        public string SortOrder { get; set; } = "desc";
    }
}