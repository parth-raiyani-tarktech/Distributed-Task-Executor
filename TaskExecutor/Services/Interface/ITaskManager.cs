using Task = TaskExecutor.Models.Task;
using TaskStatus = TaskExecutor.Models.TaskStatus;

namespace TaskExecutor.Services.Interface
{
    public interface ITaskManager
    {
        public Task CreateTask();
        public Task? GetFirstPendingTask();
        public List<Task> GetTasksByStatus(TaskStatus status);
        public List<Task> GetAllTasksInExecutionOrder();
    }
}
