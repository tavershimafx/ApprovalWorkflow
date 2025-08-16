using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
}
