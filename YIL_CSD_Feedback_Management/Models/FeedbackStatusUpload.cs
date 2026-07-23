using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIL_CSD_Feedback_Management.Models
{
    [Table("tblFeedbackStatusUpload")]
    public class FeedbackStatusUpload
    {
        [Key]
        public long UploadID { get; set; }

        public long UploadHeaderID { get; set; }

        public string CaseNumber { get; set; } = string.Empty;

        public string FeedbackStatus { get; set; } = string.Empty;

        public string? FileName { get; set; }

        public string? UploadedBy { get; set; }

        public DateTime UploadedDate { get; set; }

        public string? Remarks { get; set; }

        [ForeignKey(nameof(UploadHeaderID))]
        public virtual FeedbackStatusUploadHeader? Header { get; set; }
    }
}