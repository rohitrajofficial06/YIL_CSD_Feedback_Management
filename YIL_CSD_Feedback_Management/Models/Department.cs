using System.ComponentModel.DataAnnotations;
using YIL_CSD_Feedback_Management.Models.Common;

namespace YIL_CSD_Feedback_Management.Models
{
    public class Department : BaseEntity
    {
        [Key]
        public int DepartmentID { get; set; }

        [Required]
        [StringLength(20)]
        public string DepartmentCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 1;
    }
}