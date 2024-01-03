namespace TaskExecutor.Models
{
    public class Task
    {
        public Guid Id { get; set; }
        public TaskStatus Status { get; set; }

        public Task()
        {
            Id = Guid.NewGuid();
            Status = TaskStatus.Pending;
        }

        public void UpdateStatus(TaskStatus newStatus)
        {
            Status = newStatus;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Status);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Task otherTask))
                return false;

            return Id == otherTask.Id && Status == otherTask.Status;
        }
    }
}
