namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class PendingFeedbackViewModel
    {
        // ==========================================================
        // Filters
        // ==========================================================

        public string? SearchText { get; set; }

        public string? Region { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public long? UploadId { get; set; }


        // ==========================================================
        // Pagination
        // ==========================================================

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalRecords { get; set; }

        public int TotalPages =>
            TotalRecords == 0
                ? 0
                : (int)Math.Ceiling(
                    (double)TotalRecords / PageSize);


        // ==========================================================
        // Summary
        // ==========================================================

        public int TotalPending { get; set; }

        public int EastPending { get; set; }

        public int WestPending { get; set; }

        public int NorthPending { get; set; }

        public int SouthPending { get; set; }

        public int GujaratPending { get; set; }

        public int BHQPending { get; set; }


        // ==========================================================
        // Upload List
        // ==========================================================

        public List<PendingFeedbackUploadViewModel> Uploads { get; set; }
            = new();


        // ==========================================================
        // Records
        // ==========================================================

        public List<PendingFeedbackRecordViewModel> Records { get; set; }
            = new();
    }


    public class PendingFeedbackRecordViewModel
    {
        public int SerialNo { get; set; }

        public long DetailID { get; set; }

        public string CaseNumber { get; set; } = "";

        public string CaseOwnerOrg { get; set; } = "";

        public string Region { get; set; } = "";

        public string MatchStatus { get; set; } = "";

        public string Remarks { get; set; } = "";

        public string FileName { get; set; } = "";

        public long UploadID { get; set; }

        public string UploadedBy { get; set; } = "";

        public DateTime UploadDate { get; set; }
    }


    public class PendingFeedbackUploadViewModel
    {
        public long UploadID { get; set; }

        public string FileName { get; set; } = "";

        public string UploadedBy { get; set; } = "";

        public DateTime UploadDate { get; set; }

        public int TotalCases { get; set; }

        public int MatchedCases { get; set; }

        public int UnMatchedCases { get; set; }
    }
}