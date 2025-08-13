using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApprovalSystem.Models
{
    [Table("Approval.Users")]
    [Index(nameof(CreatedById), IsUnique = false)]
    [Index(nameof(UpdatedById), IsUnique = false)]
    public class User : IdentityUser<long>, IModelBase<long>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public long CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public long UpdatedById { get; set; }

        public User UpdatedBy { get; set; }

        public DateTimeOffset? LastUpdated { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
