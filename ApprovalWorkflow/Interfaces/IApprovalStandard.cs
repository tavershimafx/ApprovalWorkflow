using ApprovalSystem.Types;

namespace ApprovalSystem.Interfaces
{
    public interface IApprovalStandard
    {
        void GetApprovingEntity<T>(long entityId, bool includePreviousState = false) where T : class, IApprovingEntity<T>;

        void Approved(long entityId);

        void Rejected(long entityId);

        void StepChanged(long entityId, long stepId, byte stepOrder);
    }
    
}
