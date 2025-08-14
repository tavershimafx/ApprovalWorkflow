using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApprovalSystem.Models
{
    public interface IModelBase<TKey>
    {
        public TKey Id { get; set; }

        [ConcurrencyCheck]
        public string ConcurrencyStamp { get; set; }

        public EntityStatus Status { get; set; }

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
