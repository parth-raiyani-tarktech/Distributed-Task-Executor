namespace TaskExecutor.Models
{
    public class Task
    {
        public Guid Id { get; set; }
        public TaskStatus Status { get; set; }
        // REVIEW:
        //  A very minor thing, but use standard and consistent naming conventions. i.e. PascalCase for public members in C#
        public List<TaskAllocation> taskAllocations { get; set; }

        public Task()
        {
            Id = Guid.NewGuid();
            Status = TaskStatus.Pending;
            taskAllocations = new List<TaskAllocation>();
        }
    }
}
