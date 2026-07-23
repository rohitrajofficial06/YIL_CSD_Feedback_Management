using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace YIL_CSD_Feedback_Management.Areas.Admin.ViewModels
{
    public class FeedbackStatusUploadViewModel
    {
        [Required]
        public string SelectedStatus { get; set; } = "Closed";

        [Required]
        public IFormFile ExcelFile { get; set; }

        public int TotalRecords { get; set; }

        public int UpdatedRecords { get; set; }

        public int RejectedRecords { get; set; }

        public List<string> RejectedCaseNumbers { get; set; }
            = new();

        public List<SelectListItem> StatusList { get; set; }
            = new()
            {
                new SelectListItem
                {
                    Text="Closed",
                    Value="Closed"
                }
                
                //,

                //new SelectListItem
                //{
                //    Text="Pending",
                //    Value="Pending"
                //},

                //new SelectListItem
                //{
                //    Text="In-Progress",
                //    Value="In-Progress"
                //}
            };


        public List<FeedbackStatusUploadHeaderViewModel> UploadHistory { get; set; }
     = new();

        public class FeedbackStatusUploadHeaderViewModel
        {
            public long UploadHeaderID { get; set; }

            public string FileName { get; set; } = "";

            public string StatusType { get; set; } = "";

            public int TotalRecords { get; set; }

            public int UpdatedRecords { get; set; }

            public int RejectedRecords { get; set; }

            public string UploadedBy { get; set; } = "";

            public DateTime UploadedDate { get; set; }
        }
    }


    public class FeedbackStatusUploadHistoryViewModel
    {
        public long UploadID { get; set; }

        public string CaseNumber { get; set; } = string.Empty;

        public string FeedbackStatus { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string UploadedBy { get; set; } = string.Empty;

        public string Remarks { get; set; } = string.Empty;

        public DateTime UploadedDate { get; set; }
    }
}