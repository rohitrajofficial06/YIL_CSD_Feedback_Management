namespace YIL_CSD_Feedback_Management.Models.Common
{
    public abstract class BaseEntity
    {
        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; } = "System";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}