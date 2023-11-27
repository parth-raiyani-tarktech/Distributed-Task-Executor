using Quartz.Logging;
using TaskExecutor.Models;
using TaskExecutor.Services.Scheduler;

namespace TaskExecutor.Services
{
    public class NodeManager
    {
        private static readonly List<Node> Nodes = new List<Node>();
        private readonly IMyScheduler _scheduler;

        public NodeManager(IServiceProvider serviceProvider)
        {
            _scheduler = serviceProvider.GetService<IMyScheduler>();
        }

        public static Node? GetFirstAvailableNode()
        {
            return Nodes.FirstOrDefault(_ => _.Status == NodeStatus.Available);
        }

        // REVIEW:
        //   Can you help me understand why these methods are static?
        public static void UpdateNodeStatus(Node nodeToUpdate, NodeStatus status)
        {
            if (nodeToUpdate != null)
            {
                nodeToUpdate.Status = status;
                Nodes.First(_ => _.Id.Equals(nodeToUpdate.Id)).Status = status;
            }
        }

        public static void UpdateNodeStatusByName(string nodeName, NodeStatus status)
        {
            if (nodeName != null)
            {
                Nodes.First(_ => _.Name.Equals(nodeName, StringComparison.CurrentCultureIgnoreCase)).Status = status;
            }
        }

        public void RegisterNode(string name, string address)
        {
            if(name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (address == null)
            {
                throw new ArgumentException(nameof(address));
            }

            if(Nodes.Find(_ => _.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) != null)
            {
                throw new ArgumentException($"Node with {nameof(name)}: {name} already exist!");
            }

            if (Nodes.Find(_ => _.Address.Equals(address, StringComparison.OrdinalIgnoreCase)) != null)
            {
                throw new ArgumentException($"Node with Address {nameof(address)}: {address} already exist!");
            }

            Node newNode = new Node(name, address);
            Nodes.Add(newNode);

            _scheduler.Start(newNode);

            TaskAllocator.ExecuteTaskAsync();
        }

        public void UnregisterNode(string name)
        {
            Node? nodeToRemove = Nodes.FindLast(_ => _.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if(nodeToRemove != null)
            {
                _scheduler.StopAsync(nodeToRemove);
                TaskAllocator.StopOngoingTaskOf(nodeToRemove);
                Nodes.Remove(nodeToRemove);
            }
        }

        public List<Node> GetAllNodes()
        {
            return Nodes;
        }

        public Node? GetNodeByName(string name)
        {
            return Nodes.FirstOrDefault(_ => _.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void OnNodeDown(string name)
        {
            Node node = GetNodeByName(name);
            if(node != null)
            {
                UpdateNodeStatus(node, NodeStatus.Offline);
                TaskAllocator.StopOngoingTaskOf(node);
            }
        }
    }
}
