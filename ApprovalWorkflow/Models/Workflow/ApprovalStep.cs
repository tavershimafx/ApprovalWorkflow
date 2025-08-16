using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApprovalSystem.Models
{
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
}
