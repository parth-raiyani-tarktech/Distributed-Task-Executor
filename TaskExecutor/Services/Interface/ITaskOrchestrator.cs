using TaskExecutor.Models;
using Task = TaskExecutor.Models.Task;

namespace TaskExecutor.Services.Interface
{
    public interface ITaskOrchestrator
    {
        public void ExecuteTaskAsync();
        public void StopOngoingTaskOf(Node node);
    }
}
