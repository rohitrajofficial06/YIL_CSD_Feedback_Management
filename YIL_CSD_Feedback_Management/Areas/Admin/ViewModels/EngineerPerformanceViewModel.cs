namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class EngineerPerformanceViewModel
    {
        public int TotalEngineers { get; set; }

        public int TotalClosedCases { get; set; }

        public string TopEngineer { get; set; } = "";

        public int TopEngineerCases { get; set; }

        public List<EngineerPerformanceRowViewModel> Engineers { get; set; }
            = new();
    }

    public class EngineerPerformanceRowViewModel
    {
        public string EngineerName { get; set; } = "";

        public string Region { get; set; } = "";

        public int ClosedCases { get; set; }

        public DateTime LastClosedDate { get; set; }
    }
}