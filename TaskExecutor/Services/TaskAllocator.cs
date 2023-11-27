using System.Threading.Tasks;
using TaskExecutor.Models;
using Task = TaskExecutor.Models.Task;
using SystemTask = System.Threading.Tasks.Task;

namespace TaskExecutor.Services
{
    public class TaskAllocator
    {
        private static readonly List<Task> _tasks = new List<Task>();
        // REVIEW:
        //  While this is not a problem in an exercise implementation, in production, it'd be better to keep these values in appsettings. To be able to tweak it and have different values in different environments
        private static readonly int RetryCount = 3;
        private static readonly int Timeout = 10;

        public Task CreateTask()
        {
            Task task = new Task();
            _tasks.Add(task);
            ExecuteTaskAsync();
            return task;
        }

        // REVIEW:
        //  Again minor one, but as this method is returning all tasks with a given status, method name should also use plural. i.e. GetTasksByStatus
        public List<Task> GetTaskByStatus(Models.TaskStatus status)
        {
            return _tasks.Where(_ => _.Status == status).ToList();
        }

        public List<Task> GetAllTasksInExecutionOrder()
        {
            return _tasks.Where(_ => _.Status == Models.TaskStatus.Pending).ToList();
        }

        // REVIEW:

        //  I like how you learnt using async programming to solve this, especially when you are not very familiar with C# :kudos:

        //  A few improvements:
        //      I do not get why the method is `public` and `static`. Looks like this can be a private member method instead. May be I'm missing something? 
        //      Async methods with `void` return types create problems of parent class going out of scope while the task is running.
        //      Async methods should always have a return type of `Task` or `Task<>`. `Task` would be similar to void in case of an async method.

        //  Clean Code
        //    This method seems to be doing multiple things:
        //      1. Finds available node and task
        //      2. If found, updates node and task's status
        //      3. Adds/Tracks the task in allocation list
        //      4. Triggers execution of task on the agent
        //      5. Waits for the execution to complete, or timeout to be elapsed
        //      6. Updates task's status based on execution outcome
        //      7. Aborts task on the agent if timeout is elapsed

        //    As you notice in above list, this method performs all steps of Task allocation and execution. The class being TaskAllocator, it should have single responsibility, to allocate tasks to nodes.
        //      It's better to have a separate class that handles execution part of the process.
        //      You can also break this method down into multiple methods, even private methods to make it more readable. Remember an article Vimal shared about ideal code to be like a story?
        //      If the method reads something like below, it'd be very easier to read and maintain in the future:
        /*
        private async Task ExecuteTaskAsync()
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

            nodeManager.UpdateNodeStatus(...);
            task.UpdateStatus(status);

            await this.AllocateTaskToNode(task);

            // This `ExecuteTask` method would implement HTTP call and timeout mechanism and return appropriate status.
            
            var result = await taskExecutor.ExecuteTask(task, node);
            if (result == Models.TaskStatus.Completed) {
                task.UpdateStatus(...);
            }
            else
            {
                var nextStatus = ShouldRetry(task) ? Pending : Failed;
                task.UpdateStatus(nextStatus)
            }

            nodeManager.updateNodeStatus(....);
        }
        */

        //  Potential Issue:
        //    I see this method calling itself at the end, and I get why you are doing that. But this creates a chance of memory leak. Imagine a scenario where I submit 10K tasks.
        //    How many instances of this method will be running after a while?
        //    I would suggest look for other possible options to handle this scenario in a better way
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

            // REVIEW: We should move this allocation piece to a separate method to make it more cleaner. Something like `AllocateTaskToNode(task, node)` in this same class 
            //  Also, maintaining list of allocations is the job of TaskAllocator. Meaning, we should maintain list of allocated tasks per node as a state of TaskAllocator's instance instead of node's
            //  With that, you can also get rid of having to maintain the list at two places - node and task
            //  Currently, this is maintaining redundant state, same information of task allocation is tracked at two places. Redundancy in maintinaing state is always a red flag
            
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
