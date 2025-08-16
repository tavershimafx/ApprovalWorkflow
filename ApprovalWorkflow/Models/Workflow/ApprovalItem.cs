using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApprovalSystem.Models
{
    [Table("Approval.ApprovalItems")]
    public class ApprovalItem : BaseModel
    {
        [Required]
        public string Comments { get; set; }

        public ApprovalItemStatus ApprovalStatus { get; set; }

        /// <summary>
        /// A user which has performed an action on this item
        /// </summary>
        public long AuthoringUserId { get; set; }

        public User AuthoringUser { get; set; }

    }
}
