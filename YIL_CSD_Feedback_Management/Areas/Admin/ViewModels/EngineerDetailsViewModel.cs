namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class EngineerDetailsViewModel
    {
        public string EngineerName { get; set; } = "";

        public int TotalCases { get; set; }

        public DateTime? FirstClosedDate { get; set; }

        public DateTime? LastClosedDate { get; set; }

        // Search Filters

        public string? SearchCaseNumber { get; set; }

        public string? SearchCompany { get; set; }

        public string? Region { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        // Result

        public List<EngineerCaseViewModel> Cases { get; set; }
            = new();
    }

    public class EngineerCaseViewModel
    {
        public long ClosedCaseID { get; set; }

        public string CaseNumber { get; set; } = "";

        public string CompanyName { get; set; } = "";

        public string RespondentName { get; set; } = "";

        public string Region { get; set; } = "";

        public DateTime ClosedDate { get; set; }

        public string ClosedBy { get; set; } = "";
    }
}