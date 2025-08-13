using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApprovalSystem.Models
{
    /// <summary>
    /// A base helper for entities with key of type long so we avoid creating long
    /// inheritance of generics everywhere
    /// </summary>
    public abstract class BaseModel : BaseModel<long>
    {

    }

    /// <summary>
    /// The Base Model for all entities. Entities who inherit from this base model
    /// Do not need approval to be displayed to general users
    /// </summary>
    /// <typeparam name="K"></typeparam>
    [Index(nameof(CreatedById), IsUnique = false)]
    [Index(nameof(UpdatedById), IsUnique = false)]
    public abstract class BaseModel<K>: IModelBase<K>
    {
        public BaseModel()
        {
            
        }

        public K Id { get; set; }

        [ConcurrencyCheck]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString("N").Normalize();

        [ForeignKey(nameof(CreatedBy))]
        public long CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public long UpdatedById { get; set; }

        public User UpdatedBy { get; set; }

        public DateTimeOffset? LastUpdated { get; set; }
    }
}
