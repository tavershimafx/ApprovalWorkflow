namespace ApprovalSystem.Types
{
    public class TaskResult<T> : TaskResult
    {
        public T Value { get; set; }

        public int StatusCode { get; set; } = 0;

        protected internal TaskResult(T value, bool succeeded, string error): base(succeeded, error)
        {
            Value = value;
        }

        public static new TaskResult<T> Ok()
        {
            return new TaskResult<T>(default, true, null);
        }

        public static TaskResult<T> Ok(T value, IEnumerable<string> warnings = null)
        {
            return new TaskResult<T>(value, true, null)
            {
                Warnings = warnings
            };
        }

        public static new TaskResult<T> Fail(string error)
        {
            return new TaskResult<T>(default, false, error);
        }

        public static TaskResult<T> Fail(T data)
        {
            return new TaskResult<T>(data, false, default);
        }

        public static new TaskResult<T> Fail(IEnumerable<string> errors)
        {
            return new TaskResult<T>(default, false, null)
            {
                Errors = errors
            };
        }
    }
}
