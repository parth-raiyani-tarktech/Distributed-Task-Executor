using TaskExecutor.Models;
using TaskStatus = TaskExecutor.Models.TaskStatus;
using SystemTask = System.Threading.Tasks.Task;


namespace TaskExecutor.Services
{
    public class TaskExecutor
    {
        public async Task<TaskStatus> ExecuteTaskAsync(Models.Task task, Node node, int timeout)
        {
            HttpClient client = new HttpClient();

            var taskStatus = TaskStatus.Failed;
            
            CancellationTokenSource cts = new CancellationTokenSource();

            var response = SystemTask.Run(async () =>
                                    Enum.Parse<TaskStatus>(await client.GetStringAsync(node.GetExecuteTaskUrl())));

            var completedTask = await SystemTask.WhenAny(response, SystemTask.Delay(TimeSpan.FromSeconds(timeout), cts.Token));

            if (completedTask == response)
            {
                taskStatus = await response;
            }
            else
            {
                HttpRequestMessage abortTaskRequest = new HttpRequestMessage(HttpMethod.Post, node.GetAbortTaskUrl());
                var abortResponse = await client.SendAsync(abortTaskRequest);
                abortResponse.EnsureSuccessStatusCode();
            }

            return taskStatus;
        }
    }
}