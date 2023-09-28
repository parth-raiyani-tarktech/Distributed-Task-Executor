namespace TaskExecutor.Models
{
    public class Task
    {
        public Guid Id { get; set; }
        public TaskStatus Status { get; set; }
        public List<TaskAllocation> taskAllocations { get; set; }

        public Task()
        {
            Id = Guid.NewGuid();
            Status = TaskStatus.Pending;
            taskAllocations = new List<TaskAllocation>();
        }
    }
}
