using Task = TaskExecutor.Models.Task;
using TaskStatus = TaskExecutor.Models.TaskStatus;
using TaskExecutor.Repositories.Interface;

namespace TaskExecutor.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly List<Task> _tasks;

        public TaskRepository()
        {
            _tasks = new List<Task>();
        }

        public Task CreateTask(Task newTask)
        {
            _tasks.Add(newTask);
            return newTask;
        }

        public Task? GetFirstPendingTask()
        {
            return _tasks.FirstOrDefault(_ => _.Status == TaskStatus.Pending);
        }

        public List<Task> GetTasksByStatus(TaskStatus status)
        {
            return _tasks.Where(_ => _.Status == status).ToList();
        }

        public List<Task> GetAllTasksInExecutionOrder()
        {
            return _tasks.Where(_ => _.Status == TaskStatus.Pending).ToList();
        }
    }
}
