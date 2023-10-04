using Quartz;
using TaskExecutor.Models;

namespace TaskExecutor.Services
{
    public interface IMyScheduler
    {
        public System.Threading.Tasks.Task Start(Node node);
    }
}