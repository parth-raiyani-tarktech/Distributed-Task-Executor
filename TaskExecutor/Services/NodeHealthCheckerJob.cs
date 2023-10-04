using Quartz;

namespace TaskExecutor.Services
{
    public class NodeHealthCheckerJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            HttpClient client = new HttpClient();

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            string name = dataMap.GetString("name");
            string address = dataMap.GetString("address");

            try
            {
                var response = await client.GetAsync(address + "/api/Task/health-check");
                if(!response.IsSuccessStatusCode)
                {
                    NodeManager.UnregisterNode(name);
                }

            } catch (Exception ex) {
                NodeManager.UnregisterNode(name);
            }
        }
    }
}
