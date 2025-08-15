using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApprovalSystem.Models
{
    /// <summary>
    /// Approval Types are fixed, only created through DB seeding or a like process
    /// E.g Process for approving roles(RolesApproval), a process for approving transactions(TransactionApproval)
    /// This is because the system needs to be certain that an implementation for it already exists
    /// This is based on the trust that the developer manually adds it after creating the code implementation
    /// for its approval workflow
    /// </summary>
    [Table("Approval.ApprovalTypes")]
    public class ApprovalType : BaseModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [Table("Approval.ApprovalSteps")]
    public class ApprovalStep : BaseModel
    {
        /// <summary>
        /// A name tag given to the step. E.g Project Management Concurrence
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The position in the approval workflow which the step should come
        /// </summary>
        [Required]
        public byte Order { get; set; }

        [Required]
        public long RoleId { get; set; }

        public Role Role { get; set; }

        public ApprovalStepRule Rule { get; set; }

        /// <summary>
        /// The number of users required to approve the step if the rule is set to
        /// <see cref="ApprovalStepRule.AnyXUsersFromRole"/>
        /// </summary>
        public byte NumberOfUsers { get; set; }

        /// <summary>
        /// A json serialized array of user Ids which will be used if the rule is set
        /// to <see cref="ApprovalStepRule.SpecificUsers"/>
        /// </summary>
        [StringLength(500)]
        public string XUserIds { get; set; }
    }

    public enum ApprovalStepRule
    {
        /// <summary>
        /// All Users from the specified role must approve before the step can
        /// be seen as completed
        /// </summary>
        AllUsersFromRole = 1,

        /// <summary>
        /// Only a single user is required to approve the step from the role before it
        /// can proceed to the next step
        /// </summary>
        AnyUserFromRole = 2,

        /// <summary>
        /// Requires any x number of users as specified during configuration to approve the
        /// step before it proceeds to the next step
        /// </summary>
        AnyXUsersFromRole = 3,

        /// <summary>
        /// Maps the step to specific number of users to approve and all must approve before
        /// it proceeds to the next step
        /// </summary>
        SpecificUsers = 4
    }

    [Table("Approval.ApprovalRequests")]
    [Index(nameof(ApprovalTypeId), IsUnique = false)]
    public class ApprovalRequest : BaseModel
    {
        [Required]
        public long ApprovalTypeId { get; set; }

        public ApprovalType ApprovalType { get; set; }

        [Required]
        public byte CurrentStep { get; set; }

        [Required]
        public long EntityId { get; set; }
    }

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

    public enum ApprovalItemStatus
    {
        Approved = 1,

        SentBack = 2,

        Rejected = 3,
    }
}
