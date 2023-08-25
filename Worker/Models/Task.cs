namespace Worker.Models
{
    public class Task
    {
        public Guid Id { get; set; }
        public TaskStatus status { get; set; }

        public Task()
        {
            Id = Guid.NewGuid();
            status = TaskStatus.Running;
        }
    }
}
