using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YIL_CSD_Feedback_Management.Models.Common;

namespace YIL_CSD_Feedback_Management.Models
{
    [Table("tblFeedbackQuestion")]
    public class FeedbackQuestion : BaseEntity
    {
        [Key]
        public int QuestionID { get; set; }

        public int DepartmentID { get; set; }

        public int QuestionNo { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public string ControlType { get; set; } = "Rating";

        public int? MaxRating { get; set; } = 5;

        public int? MinRating { get; set; } = 1;

        public bool IsMandatory { get; set; } = true;

        public int DisplayOrder { get; set; } = 1;
        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }
    }
}