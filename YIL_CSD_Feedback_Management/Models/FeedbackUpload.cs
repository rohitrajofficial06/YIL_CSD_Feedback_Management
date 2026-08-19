using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YIL_CSD_Feedback_Management.Helpers;
using YIL_CSD_Feedback_Management.Models.Common;

namespace YIL_CSD_Feedback_Management.Models
{
    [Table("trnFeedbackUpload")]
    public class FeedbackUpload : BaseEntity
    {
        [Key]
        public long UploadID { get; set; }

        public int? DepartmentID { get; set; }

        [Required]
        [StringLength(300)]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string SavedFileName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(20)]
        public string? FileExtension { get; set; }

        [StringLength(100)]
        public string? ContentType { get; set; }

        public long FileSize { get; set; }

        [StringLength(30)]
        public string UploadStatus { get; set; } = "Uploaded";

        [StringLength(30)]
        public string OCRStatus { get; set; } = "Pending";

        public DateTime? OCRCompletedDate { get; set; }

        public string? OCRText { get; set; }


        [StringLength(100)]
        public string? TemplateName { get; set; }

        [StringLength(100)]
        public string? UploadedBy { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        public DateTime UploadedDate { get; set; } = DateTimeHelper.Now;
        
        [NotMapped]
        public string? DepartmentName { get; set; }
    }
}