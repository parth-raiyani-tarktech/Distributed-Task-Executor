using Task = TaskExecutor.Models.Task;
using TaskStatus = TaskExecutor.Models.TaskStatus;

namespace TaskExecutor.Repositories.Interface
{
    public interface ITaskRepository
    {
        public Task CreateTask(Task newTask);
        public Task? GetFirstPendingTask();
        public List<Task> GetTasksByStatus(TaskStatus status);
        public List<Task> GetAllTasksInExecutionOrder();
    }
}
