using TaskExecutor.Models;
using TaskExecutor.Repositories.Interface;

namespace TaskExecutor.Repositories
{
    public class NodeRepository : INodeRepository
    {
        private readonly List<Node> _nodes = new List<Node>();

        public Node? GetFirstAvailableNode()
        {
            return _nodes.FirstOrDefault(_ => _.Status == NodeStatus.Available);
        }

        public void RegisterNode(Node newNode)
        {
            _nodes.Add(newNode);
        }

        public void UnregisterNode(Node node)
        {
            _nodes.Remove(node);
        }

        public List<Node> GetAllNodes()
        {
            return _nodes;
        }

        public Node? GetNodeByName(string name)
        {
            return _nodes.FirstOrDefault(_ => _.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public Node? GetNodeByAddress(string address)
        {
            return _nodes.FirstOrDefault(_ => _.Address.Equals(address, StringComparison.OrdinalIgnoreCase));
        }
    }
}
