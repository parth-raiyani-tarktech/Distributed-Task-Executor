using TaskExecutor.Models;

namespace TaskExecutor.Services
{
    public class NodeManager
    {
        private static readonly List<Node> Nodes = new List<Node>();

        public static Node? GetFirstAvailableNode()
        {
            Node? availableNode = Nodes.FirstOrDefault(_ => _.Status == NodeStatus.Available);
            if(availableNode != null)
            {
                availableNode.Status = NodeStatus.Busy;
            }
            return availableNode;
        }

        public static void UpdateNodeStatusToAvailable(Node nodeToUpdate)
        {
            Nodes.First(_ => _.Equals(nodeToUpdate)).Status = NodeStatus.Available;
        }

        public void RegisterNode(string name, string address)
        {
            Nodes.Add(new Node(name, address));
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
