namespace ApprovalSystem.Models
{
    public enum EntityStatus
    {
        /// <summary>
        /// 
        /// </summary>
        Active,

        /// <summary>
        /// Indicates a record is temporarily suspended. It is not available for use, 
        /// but it is not deleted. Can only be viewed by administrators or users with special permissions.
        /// </summary>
        Suspended,

        /// <summary>
        /// This marks a permanent delete on the resource which means it has been approved to be deleted
        /// permanently but for record keeping we still keep it. Happens mostly in financial environments
        /// but it never shows up in queries whatsoever
        /// </summary>
        PermanentDelete = 7
    }
}
