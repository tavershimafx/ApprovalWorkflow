using ApprovalSystem.Dtos;
using ApprovalSystem.Services;
using ApprovalSystem.Types;

namespace ApprovalSystem.Interfaces
{
    public interface IApprovalSetup : IApplicationScopedService
    {
        /// <summary>
        /// Creates a new approval type in the system. This method is intended to be called
        /// by the DB seeding process only.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        TaskResult CreateApprovalType(ApprovalTypeModel model);

        /// <summary>
        /// Gets an overview of the type of approvals registered in the system
        /// </summary>
        /// <returns></returns>
        TaskResult<IEnumerable<ApprovalTypeItem>> GetApprovalTypes();

        /// <summary>
        /// Finds and attaches all relevant info related to that approval type
        /// returned to the caller
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns>Details of the approval type specified by the <paramref name="typeId"/></returns>
        TaskResult<ApprovalTypeDetails> GetApprovalType(long typeId);

        /// <summary>
        /// Finds the approval type which is matched to the given <paramref name="implementingInterface"/>
        /// The interface implements the necessary methods required to approve a specific model object
        /// </summary>
        /// <param name="implementingInterface"></param>
        /// <returns></returns>
        TaskResult<ApprovalTypeDetails> GetApprovalType<T>(T implementingInterface) where T:IApprovalStandard;

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
        TaskResult UpdateSteps(long typeId, IEnumerable<ApprovalStepDto> steps);

        /// <summary>
        /// Deletes the given steps from the approval type specified by the <paramref name="typeId"/>
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="stepIds"></param>
        /// <returns></returns>
        TaskResult DeleteSteps(long typeId, IEnumerable<long> stepIds);
    }
}
