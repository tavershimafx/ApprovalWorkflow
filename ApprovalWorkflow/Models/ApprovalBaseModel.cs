using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ApprovalSystem.Models
{
    /// <summary>
    /// A generic base model for all entities which need to pass through approval workflow
    /// </summary>
    /// <typeparam name="K"></typeparam>
    [Index(nameof(ApprovalHashId), IsUnique = true)]
    public abstract class ApprovalBaseModel<K> : BaseModel<K>, IApprovableEntity<K>
    {
        public ApprovalBaseModel()
        {
            ApprovalStatus = ApprovalStatus.New;
        }

        public K ShadowId { get; set; }

        public string ApprovalHashId { get; set; }

        [Required]
        public ApprovalStatus ApprovalStatus { get; set; }
    }


    /// <summary>
    /// A base helper for entities which need approval with key of type long 
    /// so we avoid creating long inheritance of generics everywhere
    /// </summary>
    public abstract class ApprovalBaseModel : ApprovalBaseModel<long>
    {

    }
}
