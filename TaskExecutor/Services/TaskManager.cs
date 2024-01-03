using System.Threading.Tasks;
using TaskExecutor.Models;
using Task = TaskExecutor.Models.Task;
using TaskStatus = TaskExecutor.Models.TaskStatus;
using TaskExecutor.Services.Interface;
using TaskExecutor.Repositories.Interface;

namespace TaskExecutor.Services
{
    public class TaskManager : ITaskManager
    {
        private readonly ITaskRepository _taskRepository;

        public TaskManager(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public Task CreateTask()
        {
            return _taskRepository.CreateTask(new Task());
        }

        public Task? GetFirstPendingTask()
        {
            return _taskRepository.GetFirstPendingTask();
        }

        public List<Task> GetTasksByStatus(TaskStatus status)
        {
            return _taskRepository.GetTasksByStatus(status);
        }

        public List<Task> GetAllTasksInExecutionOrder()
        {
            return _taskRepository.GetAllTasksInExecutionOrder();
        }
    }
}
