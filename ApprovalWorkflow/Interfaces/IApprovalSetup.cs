using ApprovalSystem.Dtos;
using ApprovalSystem.Services;
using ApprovalSystem.Types;

namespace ApprovalSystem.Interfaces
{
    public interface IApprovalSetup : IApplicationScopedService
    {
        public TaskResult<IEnumerable<ApprovalTypeItem>> GetApprovalTypes();

        public TaskResult<ApprovalTypeDetails> GetApprovalType(long typeId);
          
        public TaskResult UpdateSteps(long typeId, IEnumerable<ApprovalStepDto> steps);

        public TaskResult DeleteSteps(long typeId, IEnumerable<long> stepIds);
    }
}
