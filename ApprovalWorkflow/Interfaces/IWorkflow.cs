using ApprovalSystem.Dtos;
using ApprovalSystem.Models;
using ApprovalSystem.Services;
using ApprovalSystem.Types;

namespace ApprovalSystem.Interfaces
{
    public interface IWorkflow : IApplicationScopedService
    {
        /// <summary>
        /// Starts an approval workflow for an item to be approved by several users before
        /// it is completely integrated into the pipeline
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="implementingInterface"></param>
        /// <param name="entityId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        TaskResult StartApproval<T>(T implementingInterface, long entityId, RequestAction action) where T : IApprovalStandard;

        /// <summary>
        /// Sends the item to the client to update the entity as requested by a user in the 
        /// approval workflow
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="newState"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        TaskResult UpdateItem(long itemId, object newState, long userId);

        /// <summary>
        /// Terminates an approval request
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

        /// <summary>
        /// Approves the item requested by the user
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="comment"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        TaskResult ApproveItem(long itemId, string comment, long userId);

        /// <summary>
        /// Rejects the item requested by the user and ends the approval process
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="comment"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        TaskResult RejectItem(long itemId, string comment, long userId);

        /// <summary>
        /// Sends back the item to the previous step which it came from for the relevant
        /// users to modify and proceed approval 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="comment"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        TaskResult SendBackStep(long itemId, string comment, long userId);
    }
}
