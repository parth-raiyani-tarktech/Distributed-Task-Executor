using TaskExecutor.Models;

namespace TaskExecutor.Services
{
    public class NodeManager
    {
        private static readonly List<Node> Nodes = new List<Node>();

        public static Node? GetFirstAvailableNode()
        {
            return Nodes.FirstOrDefault(_ => _.Status == NodeStatus.Available);
        }

        public static void UpdateNodeStatus(Node nodeToUpdate, NodeStatus status)
        {
            if (nodeToUpdate != null)
            {
                nodeToUpdate.Status = NodeStatus.Busy;
                Nodes.First(_ => _.Equals(nodeToUpdate)).Status = status;
            }
            if(status == NodeStatus.Available)
            {
                TaskAllocator.ExecuteTasks();
            }
        }

        public void RegisterNode(string name, string address)
        {
            Nodes.Add(new Node(name, address));
            TaskAllocator.ExecuteTasks();
        }

        public void UnregisterNode(string name)
        {
            Node? nodeToRemove = Nodes.FindLast(_ => _.Name.ToLower().Equals(name.ToLower(), StringComparison.Ordinal));
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
