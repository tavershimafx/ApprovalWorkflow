using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApprovalSystem.Models
{
    [Table("Approval.ApprovalHashes")]
    public class ApprovalHash : BaseModel<string>
    {
        public ApprovalHash(long entityId)
        {
            EntityId = entityId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new string Id { get; set; }

        public long EntityId { get; set; }

        public ApprovalHash()
        {
            // concurrency, entityId, date, userId
            Id = Guid.NewGuid().ToString("N").Normalize();
        }
    }
}
