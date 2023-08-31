using TaskExecutor.Models;
using Task = TaskExecutor.Models.Task;

namespace TaskExecutor.Services
{
    public class TaskAllocator
    {
        private static List<Task> _tasks = new List<Task>();

        public void CreateTask(Task task)
        {
            if(task == null)
            {
                throw new ArgumentNullException("task can not be null");
            }

            task.Id = Guid.NewGuid();
            _tasks.Add(task);
        }

        public List<Task> GetTaskByStatus(Models.TaskStatus status)
        {
            return _tasks.Where(_ => _.Status == status).ToList();
        }

        public List<Task> GetAllTasksInExecutionOrder()
        {
            return _tasks;
        }
        //[TODO]: Need to properly implement Schedule all tasks
        // along with failure handle to assign it to next available node
        public async void ScheduleTasks()
        {
            Node? availableNode = NodeManager.GetFirstAvailableNode();
            Task? taskToComplete = _tasks.FirstOrDefault(_ => _.Status == Models.TaskStatus.Pending);

            if (availableNode != null && taskToComplete != null)
            {
                taskToComplete.Status = Models.TaskStatus.Running;

                HttpClient client = new HttpClient();
                var url = availableNode.Address + "/api/Task/download";

                string? response = null;
                try
                {
                    response = await client.GetFromJsonAsync<string>(url);
                } catch(Exception ex)
                {
                    response = null;
                }

                Thread.Sleep(10000);

                if(response == null)
                {
                    ScheduleTaskWithNode(NodeManager.GetFirstAvailableNode(), taskToComplete);
                    NodeManager.UpdateNodeStatusToAvailable(availableNode);
                } else
                {
                    taskToComplete.Status = Models.TaskStatus.Completed;
                    NodeManager.UpdateNodeStatusToAvailable(availableNode);
                    ScheduleTasks();
                }
                //[TODO]:api/TaskController/download - call this api with address of node and wait to 10 sec,
                //if no response, then try with another node
            }
            else if(taskToComplete == null)
            {
                return;
            } 
            else
            {
                return;//[TODO]: try after sometime as all nodes are busy
            }
        }

        public async void ScheduleTaskWithNode(Node? availableNode, Task taskToComplete)
        {
            if (availableNode != null && taskToComplete != null)
            {
                taskToComplete.Status = Models.TaskStatus.Running;

                HttpClient client = new HttpClient();
                var url = availableNode.Address + "/api/TaskController/download";

                string? response = null;
                try
                {
                    response = await client.GetFromJsonAsync<string>(url);
                }
                catch (Exception ex)
                {
                    response = null;
                }
                Thread.Sleep(1000);

                if (response == null)
                {
                    taskToComplete.Status = Models.TaskStatus.Failed;
                    NodeManager.UpdateNodeStatusToAvailable(availableNode);
                }
                else
                {
                    taskToComplete.Status = Models.TaskStatus.Completed;
                    NodeManager.UpdateNodeStatusToAvailable(availableNode);
                    ScheduleTasks();
                }
            }
            else if (taskToComplete == null)
            {
                return;
            }
            else
            {
                return;//[TODO]: try after sometime as all nodes are busy
            }
        }
    }
}
