using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApprovalSystem.Models
{
    [Table("Approval.ApprovalRequests")]
    [Index(nameof(ApprovalTypeId), IsUnique = false)]
    public class ApprovalRequest : BaseModel
    {
        public ApprovalRequest() 
        {
            RequestStatus = ApprovalItemStatus.Pending;
        }

        [Required]
        public long ApprovalTypeId { get; set; }

        public ApprovalType ApprovalType { get; set; }

        /// <summary>
        /// The current step at which the appoval is at in the workflow
        /// </summary>
        [Required]
        public byte CurrentStep { get; set; }

        /// <summary>
        /// The Id of the entity which needs to be approved. Eg ProductId, PaymentId
        /// </summary>
        [Required]
        public long EntityId { get; set; }

        /// <summary>
        /// The action which the user intends to perform on the entity
        /// </summary>
        public RequestAction RequestAction { get; set; }

        /// <summary>
        /// The current status of the request of the last modified item if it was not approved
        /// </summary>
        public ApprovalItemStatus RequestStatus { get; set; }
    }
}
