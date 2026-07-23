using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIL_CSD_Feedback_Management.Models
{
    [Table("tblClosedCaseUploadDetail")]
    public class ClosedCaseUploadDetail
    {
        [Key]
        public long DetailID { get; set; }

        public long UploadID { get; set; }

        [Required]
        [StringLength(50)]
        public string CaseNumber { get; set; } = string.Empty;

        [StringLength(300)]
        public string? CaseOwnerOrg { get; set; }



        [StringLength(50)]
        public string? Region { get; set; }

        public long? FeedbackID { get; set; }

        [StringLength(30)]
        public string? MatchStatus { get; set; }

        public DateTime? ExcelClosedDate { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [ForeignKey(nameof(UploadID))]
        public virtual ClosedCaseUpload? Upload { get; set; }
    }
}