namespace TaskExecutor.Models
{
    public class TaskAllocation
    {
        public Task Task { get; set; }
        public Node Node { get; set; }

        public TaskAllocation(Task task, Node node)
        {
            Task = task;
            Node = node;
        }
    }
}