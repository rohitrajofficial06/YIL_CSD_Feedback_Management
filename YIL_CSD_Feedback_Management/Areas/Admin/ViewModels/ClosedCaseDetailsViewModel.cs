namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class ClosedCaseDetailsViewModel
    {
        public long UploadID { get; set; }

        public string FileName { get; set; } = "";

        public string UploadedBy { get; set; } = "";

        public DateTime UploadDate { get; set; }

        public int TotalCases { get; set; }

        public bool Processed { get; set; }

        public int MatchedCases { get; set; }

        public int UnMatchedCases { get; set; }

        public int AlreadyClosedCases { get; set; }

        public int EastCases { get; set; }

        public int WestCases { get; set; }

        public int NorthCases { get; set; }

        public int SouthCases { get; set; }

        public int GujaratCases { get; set; }

        public int BHQCases { get; set; }

        public List<ClosedCaseRowViewModel> Records { get; set; }
            = new();


    }
    public class ClosedCaseRowViewModel
    {
        public long DetailID { get; set; }

        public string CaseNumber { get; set; } = "";

        public string Region { get; set; } = "";

        public string CaseOwnerOrg { get; set; } = "";

        public bool FeedbackFound { get; set; }

        public bool AlreadyClosed { get; set; }

        public long? FeedbackID { get; set; }

        public string FeedbackStatus { get; set; } = "";

        public string MatchStatus { get; set; } = "";

        public string Remarks { get; set; } = "";

        public int EastCases { get; set; }

        public int WestCases { get; set; }

        public int NorthCases { get; set; }

        public int SouthCases { get; set; }

        public int GujaratCases { get; set; }

        public int BHQCases { get; set; }
    }
}