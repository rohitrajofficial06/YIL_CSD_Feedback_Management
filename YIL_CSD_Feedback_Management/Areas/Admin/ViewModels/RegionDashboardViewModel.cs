namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class RegionDashboardViewModel
    {
        public int East { get; set; }

        public int West { get; set; }

        public int North { get; set; }

        public int South { get; set; }

        public int Gujarat { get; set; }

        public int BHQ { get; set; }

        public int Total { get; set; }

        public int TodayClosed { get; set; }

        public int ThisMonthClosed { get; set; }

        public string LastUploadedFile { get; set; } = "";

        public DateTime? LastUploadDate { get; set; }

        public List<MonthlyClosureViewModel> MonthlySummary { get; set; }
    = new();
    }

    public class MonthlyClosureViewModel
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public string MonthName { get; set; } = "";

        public int TotalCases { get; set; }
    }
}