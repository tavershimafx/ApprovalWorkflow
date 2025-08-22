namespace ApprovalSystem.Types
{
    public interface IApprovingEntity
    {
        public object NewState { get; set; }

        public object PreviousState { get; set; }
    }
}
