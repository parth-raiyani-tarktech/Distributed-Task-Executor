namespace TaskExecutor.Models
{
    public class TaskAllocation
    {
        public Task Task { get; }
        public Node Node { get; }

        public TaskAllocation(Task task, Node node)
        {
            Task = task;
            Node = node;
        }
    }
}