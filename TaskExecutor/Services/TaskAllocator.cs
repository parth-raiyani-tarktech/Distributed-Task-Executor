using TaskExecutor.Models;

namespace TaskExecutor.Services
{
    public class TaskAllocator
    {
        List<Node> Nodes = new List<Node>();

        public void RegisterNode(string name, string address)
        {
            Nodes.Add(new Node(name, address));
        }

        public void UnregisterNode(string name)
        {
            Node nodeToRemove = Nodes.FindLast(_ => _.Name.ToLower().Equals(name.ToLower(), StringComparison.Ordinal));
            if(nodeToRemove != null)
            {
                Nodes?.Remove(nodeToRemove);
            }
        }

        public void ScheduleTask()
        {
            Node availableNode = Nodes.Where(_ => _.Status == NodeStatus.Available).FirstOrDefault();

            if(availableNode != null)
            {
                //call worker to perform the task
            }
        }

        public List<Node> GetNodes()
        {
            return Nodes;
        }
    }
}
