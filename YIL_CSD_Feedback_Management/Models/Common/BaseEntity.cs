using YIL_CSD_Feedback_Management.Helpers;

namespace YIL_CSD_Feedback_Management.Models.Common
{
    public abstract class BaseEntity
    {
        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; } = "System";

        public DateTime CreatedDate { get; set; } = DateTimeHelper.Now;

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; } = DateTimeHelper.Now;
    }
}