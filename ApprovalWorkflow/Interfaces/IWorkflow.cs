using ApprovalSystem.Dtos;
using ApprovalSystem.Models;
using ApprovalSystem.Services;
using ApprovalSystem.Types;

namespace ApprovalSystem.Interfaces
{
    public interface IWorkflow : IApplicationScopedService
    {
        TaskResult StartApproval<T>(T implementingInterface, long entityId, RequestAction action) where T : IApprovalStandard;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        TaskResult CancelApproval(long requestId, long userId);

        /// <summary>
        /// Finds and returns all the approval requests currently registered in the store
        /// </summary>
        /// <returns></returns>
        TaskResult<IEnumerable<ApprovalRequestItem>> GetApprovals();

        /// <summary>
        /// Returns the details of an item in the workflow pending approval with all
        /// previous actions that have been performed on the item
        /// </summary>
        TaskResult<ApprovalRequestDetails> GetApprovalDetails(long requestId);

        TaskResult ApproveItem(long itemId, string comment, long userId);

        TaskResult RejectItem(long itemId, string comment, long userId);

        TaskResult SendBackStep(long itemId, string comment, long userId);
    }
}
