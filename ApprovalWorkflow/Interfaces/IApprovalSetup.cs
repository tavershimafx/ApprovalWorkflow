using ApprovalSystem.Dtos;
using ApprovalSystem.Services;
using ApprovalSystem.Types;

namespace ApprovalSystem.Interfaces
{
    public interface IApprovalSetup : IApplicationScopedService
    {
        /// <summary>
        /// Gets an overview of the type of approvals registered in the system
        /// </summary>
        /// <returns></returns>
        public TaskResult<IEnumerable<ApprovalTypeItem>> GetApprovalTypes();

        /// <summary>
        /// Finds and attaches all relevant info related to that approval type
        /// returned to the caller
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns>Details of the approval type specified by the <paramref name="typeId"/></returns>
        public TaskResult<ApprovalTypeDetails> GetApprovalType(long typeId);

        /// <summary>
        /// Updates existing steps, adds new steps and removes existing steps which are 
        /// no longer sent for update
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        /// If one of the steps to be updated is not found, already existing, the process
        /// terminates
        /// </exception>
        public TaskResult UpdateSteps(long typeId, IEnumerable<ApprovalStepDto> steps);

        /// <summary>
        /// Deletes the given steps from the approval type specified by the <paramref name="typeId"/>
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="stepIds"></param>
        /// <returns></returns>
        public TaskResult DeleteSteps(long typeId, IEnumerable<long> stepIds);
    }
}
