using System.Threading.Tasks;
using TaskExecutor.Models;
using Task = TaskExecutor.Models.Task;
using SystemTask = System.Threading.Tasks.Task;

namespace TaskExecutor.Services
{
    public class TaskAllocator
    {
        private static readonly List<Task> _tasks = new List<Task>();
        private static readonly int RetryCount = 3;
        private static readonly int Timeout = 10;

        public Task CreateTask()
        {
            Task task = new Task();
            _tasks.Add(task);
            ExecuteTaskAsync();
            return task;
        }

        public List<Task> GetTaskByStatus(Models.TaskStatus status)
        {
            return _tasks.Where(_ => _.Status == status).ToList();
        }

        public List<Task> GetAllTasksInExecutionOrder()
        {
            return _tasks.Where(_ => _.Status == Models.TaskStatus.Pending).ToList();
        }

        public static async void ExecuteTaskAsync()
        {
            Task? taskToComplete = _tasks
                .FirstOrDefault(_ => _.Status == Models.TaskStatus.Pending);

            Node? availableNode = NodeManager.GetFirstAvailableNode();

            if (taskToComplete == null || availableNode == null)
            {
                return;
                /* Cases:
                 * 1. No tasks to perform
                 * 2. Try after sometime as all nodes are busy
                 */
            }

            NodeManager.UpdateNodeStatus(availableNode, NodeStatus.Busy);
            taskToComplete.Status = Models.TaskStatus.Running;

            TaskAllocation taskAllocation = new TaskAllocation(taskToComplete, availableNode);
            taskToComplete.taskAllocations.Add(taskAllocation);
            availableNode.TaskAllocation.Add(taskAllocation);

            HttpClient client = new HttpClient();

            var taskStatus = Models.TaskStatus.Completed;
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();

                var response = SystemTask.Run(async () =>
                                        Enum.Parse<Models.TaskStatus>(await client.GetStringAsync(availableNode.ExecuteTaskUrl)));

                var completedTask = await SystemTask.WhenAny(response, SystemTask.Delay(TimeSpan.FromSeconds(Timeout), cts.Token));

                if (completedTask == response)
                {
                    taskStatus = await response;
                }
                else
                {
                    taskStatus = Models.TaskStatus.Failed;

                    HttpRequestMessage abortTaskRequest = new HttpRequestMessage(HttpMethod.Post, availableNode.AbortTaskUrl);
                    var abortResponse = await client.SendAsync(abortTaskRequest);
                    abortResponse.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                taskStatus = Models.TaskStatus.Failed;
            }

            if (taskStatus == Models.TaskStatus.Failed)
            {
                taskToComplete.Status = taskToComplete.taskAllocations.Count() >= RetryCount
                    ? Models.TaskStatus.Failed : Models.TaskStatus.Pending;
            } else
            {
                taskToComplete.Status = taskStatus;
            }

            NodeManager.UpdateNodeStatus(availableNode, NodeStatus.Available);
            ExecuteTaskAsync();
        }

        public static void StopOngoingTaskOf(Node node)
        {
            Task? taskToComplete = _tasks
                .Where(_ => _.Status == Models.TaskStatus.Running)
                .FirstOrDefault( _ => _.taskAllocations.Last().Node.Name == node.Name);

            taskToComplete.Status = Models.TaskStatus.Pending;
            ExecuteTaskAsync();
        }
    }
}
