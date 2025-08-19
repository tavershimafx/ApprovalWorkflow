namespace ApprovalSystem.Models
{
    /// <summary>
    /// A flag used to track the status of a shadow item which was requested for approval.
    /// </summary>
    public enum ApprovalStatus
    {
        /// <summary>
        /// Indicates that the resource is new and has not yet been approved.
        /// </summary>
        New = 1,

        /// <summary>
        /// Indicates that available for public use. Definitely it has gone through the 
        /// approval process and has been approved.
        /// </summary>
        Active = 2,

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
        /// Indicates that the resource has been deleted. 
        /// This is a soft delete, meaning the resource is not removed from the database
        /// but marked as deleted pending approval
        /// </summary>
        Deleted = 6
    }
}
