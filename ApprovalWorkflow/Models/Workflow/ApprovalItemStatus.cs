namespace ApprovalSystem.Models
{
    public enum ApprovalItemStatus
    {
        /// <summary>
        /// Indicates that the item is new and still undergoing approval in the approval workflow.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Indicates that the item has been approved.
        /// </summary>
        Approved = 2,

        /// <summary>
        /// Indicates that the item was sent back to a previous step
        /// </summary>
        SentBack = 3,

        /// <summary>
        /// Indicates that the item has been rejected through the approval process.
        /// Can be resumed later if the user wants to try again or make changes.
        /// </summary>
        Rejected = 4,

        /// <summary>
        /// Indicates that the item has been terminated by an authority and cannot be resumed.
        /// </summary>
        Terminated = 5,
    }
}
