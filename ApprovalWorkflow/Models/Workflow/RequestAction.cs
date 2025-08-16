namespace ApprovalSystem.Models
{
    public enum RequestAction
    {
        /// <summary>
        /// The item is new and is requested to be added to the main store
        /// </summary>
        New = 1,

        /// <summary>
        /// The item is an update to an already existing entity
        /// </summary>
        Update = 2,

        /// <summary>
        /// The item is to be deleted from the store
        /// </summary>
        Delete = 3
    }
}
