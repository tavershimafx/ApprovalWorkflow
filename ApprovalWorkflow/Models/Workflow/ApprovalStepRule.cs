namespace ApprovalSystem.Models
{
    public enum ApprovalStepRule
    {
        /// <summary>
        /// All Users from the specified role must approve before the step can
        /// be seen as completed
        /// </summary>
        AllUsersFromRole = 1,

        /// <summary>
        /// Only a single user is required to approve the step from the role before it
        /// can proceed to the next step
        /// </summary>
        AnyUserFromRole = 2,

        /// <summary>
        /// Requires any x number of users as specified during configuration to approve the
        /// step before it proceeds to the next step
        /// </summary>
        AnyXUsersFromRole = 3,

        /// <summary>
        /// Maps the step to specific number of users to approve and all must approve before
        /// it proceeds to the next step
        /// </summary>
        SpecificUsers = 4
    }
}
