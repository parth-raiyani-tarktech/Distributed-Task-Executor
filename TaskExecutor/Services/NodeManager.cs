using TaskExecutor.Models;
using TaskExecutor.Repositories.Interface;
using TaskExecutor.Services.Interface;
using TaskExecutor.Services.Scheduler;

namespace TaskExecutor.Services
{
    public class NodeManager : INodeManager
    {
        private readonly INodeRepository _nodeRepository;
        private readonly IMyScheduler? _scheduler;

        public NodeManager(IServiceProvider serviceProvider, INodeRepository nodeRepository)
        {
            _scheduler = serviceProvider.GetService<IMyScheduler>();
            _nodeRepository = nodeRepository;
        }

        public Node? GetFirstAvailableNode()
        {
            return _nodeRepository.GetFirstAvailableNode();
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

            if (_nodeRepository.GetNodeByName(name) != null)
            {
                throw new ArgumentException($"Node with {nameof(name)}: {name} already exist!");
            }

            if (_nodeRepository.GetNodeByAddress(address) != null)
            {
                throw new ArgumentException($"Node with Address {nameof(address)}: {address} already exist!");
            }

            Node newNode = new Node(name, address);
            _nodeRepository.RegisterNode(newNode);

            _scheduler?.Start(newNode);
        }

        public void UnregisterNode(Node nodeToRemove)
        {
            if (nodeToRemove != null)
            {
                nodeToRemove.UpdateNodeStatus(NodeStatus.Offline);
                
                _scheduler?.StopAsync(nodeToRemove);
                _nodeRepository.UnregisterNode(nodeToRemove);
            }
        }

        public List<Node> GetAllNodes()
        {
            return _nodeRepository.GetAllNodes();
        }

        public Node? GetNodeByName(string name)
        {
            return _nodeRepository.GetNodeByName(name);
        }

        public Node? GetNodeByAddress(string address)
        {
            return _nodeRepository.GetNodeByAddress(address);
        }

        public void OnNodeDown(Node node)
        {
            if (node != null)
            {
                node.UpdateNodeStatus(NodeStatus.Offline);
            }
        }
    }
}
