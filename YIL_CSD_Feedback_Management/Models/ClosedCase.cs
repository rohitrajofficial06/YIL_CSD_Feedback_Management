using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIL_CSD_Feedback_Management.Models
{
    [Table("tblClosedCases")]
    public class ClosedCase
    {
        [Key]
        public long ClosedCaseID { get; set; }

        public long? FeedbackID { get; set; }

        public long UploadID { get; set; }

        [Required]
        [StringLength(50)]
        public string CaseNumber { get; set; } = string.Empty;

        [StringLength(50)]
        public string Region { get; set; } = string.Empty;

        public DateTime ClosedDate { get; set; }

        [StringLength(100)]
        public string? ClosedBy { get; set; }
    }
}