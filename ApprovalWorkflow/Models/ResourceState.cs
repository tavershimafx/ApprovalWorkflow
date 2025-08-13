namespace ApprovalSystem.Models
{
    public enum ResourceState
    {
        /// <summary>
        /// Indicates that available for public use. Definitely it has gone through the 
        /// approval process and has been approved.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Indicates that the resource is new and has not yet been approved.
        /// </summary>
        New = 2,

        /// <summary>
        /// Indicates an existing record has been modified is pending approval.
        /// It is not available for use until it is approved.
        /// </summary>
        Modified = 3,

        /// <summary>
        /// Indicates that the resource has been rejected through the approval process,
        /// </summary>
        Rejected = 4,

        /// <summary>
        /// Indicates a record is temporarily suspended. It is not available for use, 
        /// but it is not deleted. Can only be viewed by administrators or users with special permissions.
        /// </summary>
        Suspended = 5,

        /// <summary>
        /// Indicates that the resource has been deleted. 
        /// This is a soft delete, meaning the resource is not removed from the database
        /// but marked as deleted. The resource might never show up in any queries or lists
        /// </summary>
        Deleted = 6
    }
}
