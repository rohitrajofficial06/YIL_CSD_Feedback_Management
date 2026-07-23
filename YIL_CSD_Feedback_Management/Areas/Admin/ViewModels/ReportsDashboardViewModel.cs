namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class ReportsDashboardViewModel
    {
        public int TotalUploads { get; set; }

        public int TotalClosedCases { get; set; }

        public int PendingFeedbacks { get; set; }

        public int TotalRegions { get; set; }

        public int TotalEngineers { get; set; }

        public int TodayClosedCases { get; set; }

        public int ThisMonthClosedCases { get; set; }
    }
}