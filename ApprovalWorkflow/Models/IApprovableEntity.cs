using System.ComponentModel.DataAnnotations;

namespace ApprovalSystem.Models
{
    public interface IApprovableEntity<TKey> : IModelBase<TKey>
    {
        /// <summary>
        /// ShadowId is used to track the original resource it is attempting to replace.
        /// For entities which have been approved, this property will be null.
        /// </summary>
        public TKey ShadowId { get; set; }

        public string ApprovalHashId { get; set; }

        [Required]
        public ApprovalStatus ApprovalStatus { get; set; }
    }
}
