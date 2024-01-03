namespace TaskExecutor.Models
{
    public class Node
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Address { get; } 

        public NodeStatus Status { get; set; }

        public Node(string name, string address)
        {
            Id = Guid.NewGuid();
            Name = name;
            Address = address;
            Status = NodeStatus.Available;
        }

        public string GetExecuteTaskUrl()
        {
            return Address + "/api/Task/execute";
        }

        public string GetAbortTaskUrl()
        {
            return Address + "/api/Task/abort";
        }

        public string GetHealthCheckAPI()
        {
            return Address + "/api/Task/health-check";
        }

        public void UpdateNodeStatus(NodeStatus newStatus)
        {
            Status = newStatus;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Address, Status);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Node otherNode))
                return false;

            return Id == otherNode.Id 
                && Name == otherNode.Name 
                && Address == otherNode.Address 
                && Status == otherNode.Status;
        }
    }
}
