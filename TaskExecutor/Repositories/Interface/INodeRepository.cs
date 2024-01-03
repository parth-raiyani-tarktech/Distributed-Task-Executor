using TaskExecutor.Models;

namespace TaskExecutor.Repositories.Interface
{
    public interface INodeRepository
    {
        public Node? GetFirstAvailableNode();
        public void RegisterNode(Node newNode);
        public void UnregisterNode(Node node);
        public List<Node> GetAllNodes();
        public Node? GetNodeByName(string name);
        public Node? GetNodeByAddress(string address);
    }
}
