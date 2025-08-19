using ApprovalSystem.Services;
using ApprovalSystem.Types;

namespace ApprovalSystem.Interfaces
{
    public interface IWorkflow : IApplicationScopedService
    {
        TaskResult StartApproval(long approvaTypeId, long entityId);

        TaskResult CancelApproval();

        /// <summary>
        /// Returns all the approvals pending in the workflow
        /// </summary>
        TaskResult GetApprovals();

        /// <summary>
        /// Returns the details of an item in the workflow pending approval with all
        /// previous actions that have been performed on the item
        /// </summary>
        TaskResult GetApprovalDetails(long approvalId);

        TaskResult ApproveItem();

        TaskResult RejectItem(long itemId);

        TaskResult SendBackStep();
    }
}
