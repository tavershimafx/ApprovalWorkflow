using ApprovalSystem.Types;

namespace ApprovalSystem.Interfaces
{
    public interface IApprovalStandard
    {
        T GetApprovingEntity<T>(long entityId, bool includePreviousState = false) where T : class, IApprovingEntity;

        void UpdateEntity(long entityId, string hashId);

        void OnApproved(long entityId, string hashId);

        void OnRejected(long entityId, string hashId);

        void OnStepChanged(long entityId, byte currentStep, int totalSteps);
    }
    
}
