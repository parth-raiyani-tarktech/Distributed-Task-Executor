using Quartz;
using TaskExecutor.Models;
using SystemTask = System.Threading.Tasks.Task;

namespace TaskExecutor.Services.Job
{
    public class NodeHealthCheckerJob : IJob
    {
        public async SystemTask Execute(IJobExecutionContext context)
        {
            HttpClient client = new HttpClient();

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            string? name = dataMap.GetString("name");
            string? address = dataMap.GetString("address");

            try
            {
                var response = await client.GetAsync(address + "/api/Task/health-check");
                if (!response.IsSuccessStatusCode)
                {
                    NodeManager.UpdateNodeStatusByName(name, NodeStatus.Offline);
                } else
                {
                    NodeManager.UpdateNodeStatusByName(name, NodeStatus.Available);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred! {ex.Message}");
                NodeManager.UpdateNodeStatusByName(name, NodeStatus.Offline);
            }
        }
    }
}
