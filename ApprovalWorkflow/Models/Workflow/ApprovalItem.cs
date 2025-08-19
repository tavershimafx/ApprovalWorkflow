using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApprovalSystem.Models
{
    [Table("Approval.ApprovalItems")]
    [Index(nameof(AuthoringUserId), IsUnique = false)]
    [Index(nameof(ApprovalRequestId), IsUnique = false)]
    [Index(nameof(ApprovalStepId), IsUnique = false)]
    public class ApprovalItem : BaseModel
    {
        public string Comments { get; set; }

        public ApprovalItemStatus ApprovalStatus { get; set; }

        /// <summary>
        /// The step which this item is meant to contribute in approving
        /// </summary>
        public long ApprovalStepId { get; set; }

        public ApprovalStep ApprovalStep { get; set; }

        /// <summary>
        /// A user which has performed an action on this item
        /// </summary>
        public long AuthoringUserId { get; set; }

        public User AuthoringUser { get; set; }

        /// <summary>
        /// The request which this item is tied to
        /// </summary>
        public long ApprovalRequestId { get; set; }

        public ApprovalRequest ApprovalRequest { get; set; }
    }
}
