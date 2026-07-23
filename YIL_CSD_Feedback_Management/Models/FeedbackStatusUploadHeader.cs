using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIL_CSD_Feedback_Management.Models
{
    [Table("tblFeedbackStatusUploadHeader")]
    public class FeedbackStatusUploadHeader
    {
        [Key]
        public long UploadHeaderID { get; set; }

        public string FileName { get; set; } = "";

        public string StatusType { get; set; } = "";

        public int TotalRecords { get; set; }

        public int UpdatedRecords { get; set; }

        public int RejectedRecords { get; set; }

        public string? UploadedBy { get; set; }

        public DateTime UploadedDate { get; set; }

        public virtual ICollection<FeedbackStatusUpload> Details { get; set; }
            = new List<FeedbackStatusUpload>();
    }
}