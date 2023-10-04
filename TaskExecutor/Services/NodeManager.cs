using Quartz.Logging;
using TaskExecutor.Models;

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

        public static void UpdateNodeStatus(Node nodeToUpdate, NodeStatus status)
        {
            if (nodeToUpdate != null)
            {
                nodeToUpdate.Status = status;
                Nodes.First(_ => _.Id.Equals(nodeToUpdate.Id)).Status = status;
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

        public static void UnregisterNode(string name)
        {
            Node? nodeToRemove = Nodes.FindLast(_ => _.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            TaskAllocator.OnNodeUnregister(nodeToRemove);

            if(nodeToRemove != null)
            {
                Nodes.Remove(nodeToRemove);
            }
        }

        public List<Node> GetAllNodes()
        {
            return Nodes;
        }
    }
}
