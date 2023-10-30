using Quartz;
using TaskExecutor.Models;
using SystemTask = System.Threading.Tasks.Task;

namespace TaskExecutor.Services.Scheduler
{
    public interface IMyScheduler
    {
        public SystemTask Start(Node node);
        public SystemTask StopAsync(Node node);
    }
}