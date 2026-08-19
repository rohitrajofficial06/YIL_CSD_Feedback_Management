using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIL_CSD_Feedback_Management.Models
{
    [Table("tblUserActiveSession")]
    public class UserActiveSession
    {
        [Key]
        public long SessionID { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        public Guid SessionToken { get; set; }

        public DateTime LoginDate { get; set; }

        public DateTime LastActivityDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}