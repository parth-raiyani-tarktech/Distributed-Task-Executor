using TaskExecutor.Models;
using Task = TaskExecutor.Models.Task;
using TaskStatus = TaskExecutor.Models.TaskStatus;
using TaskExecutor.Services.Interface;

namespace TaskExecutor.Services
{
    public class TaskOrchestrator : ITaskOrchestrator
    {
        private readonly ITaskManager _taskManager;
        private readonly INodeManager _nodeManager;
        
        private readonly TaskExecutor _taskExecutor;
        private readonly TaskAllocator _taskAllocator;
        
        private readonly int _retryCount;
        private readonly int _timeout;
        
        public TaskOrchestrator(IConfiguration configuration, ITaskManager taskManager, INodeManager nodeManager)
        {
            _retryCount = configuration.GetValue<int>("RetryCount");
            _timeout = configuration.GetValue<int>("Timeout");

            _taskManager = taskManager;
            _nodeManager = nodeManager;

            _taskExecutor = new TaskExecutor();
            _taskAllocator = new TaskAllocator();
        }

        public async void ExecuteTaskAsync()
        {
            var task = GetTaskToExecuteFromQueue();
            if (task == null)
            {
                Console.WriteLine("No pending tasks to execute.");
                return;
            }
            var node = GetFirstAvailableNode();
            if (node == null)
            {
                Console.WriteLine("All nodes are busy right now. Try again later.");
                return;
            }

            node.UpdateNodeStatus(NodeStatus.Busy);
            task.UpdateStatus(TaskStatus.Running);

            _taskAllocator.AllocateTaskToNode(task, node);
            var result = await _taskExecutor.ExecuteTaskAsync(task, node, _timeout);
            if (result == TaskStatus.Completed)
            {
                task.UpdateStatus(result);
            }
            else
            {
                var nextStatus = ShouldRetry(task) ? TaskStatus.Pending : TaskStatus.Failed;
                task.UpdateStatus(nextStatus);
            }

            node.UpdateNodeStatus(NodeStatus.Available);
            ExecuteTaskAsync();
        }

        private bool ShouldRetry(Task task)
        {
            return _taskAllocator.GetNodeCountWithTask(task) < _retryCount;
        }

        private Task? GetTaskToExecuteFromQueue()
        {
            return _taskManager.GetFirstPendingTask();
        }

        private Node? GetFirstAvailableNode()
        {
            return _nodeManager.GetFirstAvailableNode();
        }

        public void StopOngoingTaskOf(Node node)
        {
            var onGoingTask = _taskAllocator.GetLastExecutedTaskOf(node);
            onGoingTask.UpdateStatus(TaskStatus.Failed);
        }
    }
}
