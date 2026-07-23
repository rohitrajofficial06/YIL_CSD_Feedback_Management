using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIL_CSD_Feedback_Management.Models
{
    [Table("tblClosedCaseUpload")]
    public class ClosedCaseUpload
    {
        [Key]
        public long UploadID { get; set; }

        [Required]
        [StringLength(300)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(700)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(100)]
        public string? UploadedBy { get; set; }

        public DateTime UploadDate { get; set; }

        public int TotalCases { get; set; }

        public int MatchedCases { get; set; }

        public int UnMatchedCases { get; set; }

        public bool Processed { get; set; }

        public DateTime? ProcessedDate { get; set; }

        public virtual ICollection<ClosedCaseUploadDetail> Details { get; set; }
            = new List<ClosedCaseUploadDetail>();
    }
}