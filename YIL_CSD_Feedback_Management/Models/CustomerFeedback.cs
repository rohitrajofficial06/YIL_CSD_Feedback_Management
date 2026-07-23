using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YIL_CSD_Feedback_Management.Models.Common;

namespace YIL_CSD_Feedback_Management.Models
{
    [Table("trnCustomerFeedback")]
    public class CustomerFeedback : BaseEntity
    {
        [Key]
        public long FeedbackID { get; set; }

        public string? CaseNumber { get; set; }

        public string FeedbackStatus { get; set; } = "Open";
        public int DepartmentID { get; set; }

        [Required]
        [StringLength(20)]
        public string FeedbackSource { get; set; } = "Online";

        [Required]
        [StringLength(300)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string RespondentName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Designation { get; set; }

        [StringLength(50)]
        public string? ContactNo { get; set; }

        [StringLength(200)]
        public string? EmailID { get; set; }

        [StringLength(100)]
        public string? ServiceRequestNo { get; set; }

        [StringLength(200)]
        public string? InstrumentCategory { get; set; }

        [StringLength(200)]
        public string? YILEngineer { get; set; }

        public string? Comments { get; set; }

        [StringLength(500)]
        public string? SignaturePath { get; set; }

        public long? UploadID { get; set; }

        public bool IsSubmitted { get; set; } = true;

        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }


        public virtual ICollection<CustomerFeedbackRating> Ratings { get; set; }
            = new List<CustomerFeedbackRating>();
    }
}