namespace ApprovalSystem.Types
{
    public abstract class BaseResult
    {
        public bool Succeeded { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public IEnumerable<string> Warnings { get; set; }
    }
}
