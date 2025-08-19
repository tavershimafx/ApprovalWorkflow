namespace ApprovalSystem.Types
{
    public interface IApprovingEntity<T> where T : class
    {
        public T CurrentState { get; set; }

        public T PreviousState { get; set; }
    }
}
