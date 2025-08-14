using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApprovalSystem.Models
{
    [Table("Approval.Roles")]
    [Index(nameof(CreatedById), IsUnique = false)]
    [Index(nameof(UpdatedById), IsUnique = false)]
    [Index(nameof(ApprovalHashId), IsUnique = true)]
    public class Role: IdentityRole<long>, IApprovableEntity<long>
    {
        public Role() : base()
        {
            Status = EntityStatus.Active;
        }

        public Role(string roleName) : base(roleName)
        {
        }
        
        public string Description { get; set; }

        public long ShadowId { get; set; }

        public EntityStatus Status { get; set; }

        public string ApprovalHashId { get; set; }

        [Required]
        public ApprovalStatus ApprovalStatus { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public long? CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public long? UpdatedById { get; set; }

        public User UpdatedBy { get; set; }

        public DateTimeOffset? LastUpdated { get; set; }
    }
}
