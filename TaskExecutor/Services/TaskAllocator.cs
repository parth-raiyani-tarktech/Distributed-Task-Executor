using TaskExecutor.Models;
using Task = TaskExecutor.Models.Task;

namespace TaskExecutor.Services
{
    public class TaskAllocator
    {
        private readonly Dictionary<Node, List<Task>> _taskAllocation = new Dictionary<Node, List<Task>>();

        public void AllocateTaskToNode(Task task, Node node)
        {
            if (!_taskAllocation.ContainsKey(node))
            {
                _taskAllocation[node] = new List<Task>();
            }
            _taskAllocation[node].Add(task);
        }

        public int GetNodeCountWithTask(Task targetTask)
        {
            return _taskAllocation.Count(pair => pair.Value.Contains(targetTask));
        }

        public Task GetLastExecutedTaskOf(Node node)
        {
            return _taskAllocation[node].Last();
        }
    }
}
