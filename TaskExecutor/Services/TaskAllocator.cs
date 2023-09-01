using TaskExecutor.Models;
using Task = TaskExecutor.Models.Task;

namespace TaskExecutor.Services
{
    public class TaskAllocator
    {
        private static readonly List<Task> _tasks = new List<Task>();

        public void CreateTask(Task task)
        {
            if(task == null)
            {
                throw new ArgumentNullException("task can not be null");
            }

            task.Id = Guid.NewGuid();
            _tasks.Add(task);
            ExecuteTasks();
        }

        public List<Task> GetTaskByStatus(Models.TaskStatus status)
        {
            return _tasks.Where(_ => _.Status == status).ToList();
        }

        public List<Task> GetAllTasksInExecutionOrder()
        {
            return _tasks.Where(_ => _.Status == Models.TaskStatus.Pending ||
                    _.Status != Models.TaskStatus.Failed).ToList();
        }

        public static async void ExecuteTasks()
        {
            Node? availableNode = NodeManager.GetFirstAvailableNode();
            Task? taskToComplete = _tasks
                .FirstOrDefault(_ => _.Status == Models.TaskStatus.Pending 
                    || _.Status == Models.TaskStatus.Failed);

            if (availableNode != null && taskToComplete != null)
            {
                NodeManager.UpdateNodeStatus(availableNode, NodeStatus.Busy);
                taskToComplete.Status = Models.TaskStatus.Running;

                HttpClient client = new HttpClient();
                var url = availableNode.Address + "/api/Task/execute";

                Models.TaskStatus response = Models.TaskStatus.Completed;
                try
                {
                    response = Enum.Parse<Models.TaskStatus>(await client.GetStringAsync(url));
                } catch(Exception ex)
                {
                    response = Models.TaskStatus.Failed;
                }

                taskToComplete.Status = response;
                NodeManager.UpdateNodeStatus(availableNode, NodeStatus.Available);
            }
            else if(taskToComplete == null)
            {
                return;//No tasks to perform
            } 
            else
            {
                return;//[TODO]: try after sometime as all nodes are busy
            }
        }
    }
}
