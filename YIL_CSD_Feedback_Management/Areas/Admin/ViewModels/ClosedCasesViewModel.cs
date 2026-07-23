namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class ClosedCasesViewModel
    {
        public string? SearchCaseNumber { get; set; }

        public string? SearchEngineer { get; set; }

        public string? Region { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
        public List<ClosedCaseListViewModel> Cases { get; set; }
            = new();

        public int TotalClosedCases { get; set; }

        public int TodayClosedCases { get; set; }

        public int ThisMonthClosedCases { get; set; }
    }

    public class ClosedCaseListViewModel
    {
        public long ClosedCaseID { get; set; }

        public string CaseNumber { get; set; } = "";

        public string Region { get; set; } = "";

        public string CompanyName { get; set; } = "";

        public string RespondentName { get; set; } = "";

        public string EngineerName { get; set; } = "";

        public DateTime ClosedDate { get; set; }

        public string ClosedBy { get; set; } = "";
    }
}