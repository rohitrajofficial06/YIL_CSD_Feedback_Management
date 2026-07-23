using Microsoft.AspNetCore.Mvc.Rendering;

namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class QuestionIndexViewModel
    {
        //========================
        // Search
        //========================

        public string? SearchText { get; set; }

        public int? DepartmentID { get; set; }


        public List<SelectListItem> Departments { get; set; }
            = new();

        //========================
        // Sorting
        //========================

        public string SortColumn { get; set; } = "DisplayOrder";

        public string SortDirection { get; set; } = "asc";

        //========================
        // Paging
        //========================

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalRecords { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalRecords / PageSize);

        //========================
        // Grid
        //========================

        public List<QuestionListViewModel> Questions { get; set; }
            = new();
    }
}