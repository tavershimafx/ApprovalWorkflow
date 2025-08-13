namespace ApprovalSystem.Types
{
    public class TaskResult: BaseResult
    {
        public TaskResult(bool succeeded, string error)
        {
            Succeeded = succeeded;
            Errors = [error];
        }

        public static TaskResult Ok()
        {
            return new TaskResult(true, null);
        }

        public static TaskResult Ok(string warning = null)
        {
            return new TaskResult(true, null)
            {
                Warnings = [warning]
            };
        }

        public static TaskResult Fail(string error)
        {
            return new TaskResult(false, error);
        }

        public static TaskResult Fail(IEnumerable<string> errors)
        {
            return new TaskResult(false, null)
            {
                Errors = errors
            };
        }
    }
}
