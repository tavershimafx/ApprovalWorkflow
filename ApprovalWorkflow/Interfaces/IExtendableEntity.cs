namespace ApprovalSystem.Interfaces
{
    public interface IExtendableEntity
    {
        /// <summary>
        /// A json serializable property which could be used to store on demand light weight data about an object without 
        /// modifying the objects structure.
        /// </summary>
        public string ExtensionProperty { get; set; }
    }
}
