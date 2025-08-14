using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApprovalSystem.Models
{
    [Table("Approval.UserClaims")]
    [Index(nameof(CreatedById), IsUnique = false)]
    [Index(nameof(UpdatedById), IsUnique = false)]
    public class UserClaim : IdentityUserClaim<long>, IModelBase<long>
    {
        public UserClaim() : base()
        {
            Status = EntityStatus.Active;
        }

        public new long Id { get; set; }

        public EntityStatus Status { get; set; }

        public string ConcurrencyStamp { get; set; }

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
