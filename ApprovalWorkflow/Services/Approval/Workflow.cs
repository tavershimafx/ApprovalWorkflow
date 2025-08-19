using ApprovalSystem.Interfaces;
using ApprovalSystem.Types;

namespace ApprovalSystem.Services
{
    public class Workflow : IWorkflow
    {
        public TaskResult ApproveItem()
        {
            throw new NotImplementedException();
        }

        public TaskResult CancelApproval()
        {
            throw new NotImplementedException();
        }

        public TaskResult GetApprovalDetails(long approvalId)
        {
            throw new NotImplementedException();
        }

        public TaskResult GetApprovals()
        {
            throw new NotImplementedException();
        }

        public TaskResult RejectItem(long itemId)
        {
            throw new NotImplementedException();
        }

        public TaskResult SendBackStep()
        {
            throw new NotImplementedException();
        }

        public TaskResult StartApproval(long approvaTypeId, long entityId)
        {
            throw new NotImplementedException();
        }
    }
}
