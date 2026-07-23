using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIL_CSD_Feedback_Management.Models
{
    [Table("trnCustomerFeedbackRating")]
    public class CustomerFeedbackRating
    {
        [Key]
        public long RatingID { get; set; }

        public long FeedbackID { get; set; }

        public int QuestionNo { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        public int? RatingValue { get; set; }

        public bool IsNotApplicable { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey(nameof(FeedbackID))]
        public virtual CustomerFeedback Feedback { get; set; } = null!;
    }
}