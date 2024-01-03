using TaskExecutor.Models;

namespace TaskExecutor.Services.Interface
{
    public interface INodeManager
    {
        public Node? GetFirstAvailableNode();
        public void RegisterNode(string name, string address);
        public void UnregisterNode(Node node);
        public List<Node> GetAllNodes();
        public Node? GetNodeByName(string name);
        public Node? GetNodeByAddress(string address);
        public void OnNodeDown(Node node);
    }
}
