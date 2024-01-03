using Quartz;
using TaskExecutor.Models;
using TaskExecutor.Services.Interface;
using SystemTask = System.Threading.Tasks.Task;

namespace TaskExecutor.Services.Job
{
    public class NodeHealthCheckerJob : IJob
    {
        private readonly INodeManager _nodeManager;

        public NodeHealthCheckerJob(INodeManager nodeManager)
        {
            _nodeManager = nodeManager;
        }

        public async SystemTask Execute(IJobExecutionContext context)
        {
            HttpClient client = new HttpClient();
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            var nodeName = dataMap.GetString("name");
            var node = _nodeManager.GetNodeByName(nodeName) ?? throw new Exception("Invalid Scheduled job, no node is present!");

            try
            {
                var response = await client.GetAsync(node.GetHealthCheckAPI());
                if (!response.IsSuccessStatusCode)
                {
                    node.UpdateNodeStatus(NodeStatus.Offline);
                }
                else
                {
                    node.UpdateNodeStatus(NodeStatus.Available);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred! {ex.Message}");
                node.UpdateNodeStatus(NodeStatus.Offline);
            }
        }
    }
}
