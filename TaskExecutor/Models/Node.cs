namespace TaskExecutor.Models
{
    public class Node
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ExecuteTaskUrl { get; set; }
        public string AbortTaskUrl { get; set; }
        public NodeStatus Status { get; set; }

        public List<TaskAllocation> TaskAllocation { get; set; }

        public Node(string name, string address)
        {
            Id = Guid.NewGuid();
            Name = name;
            Address = address;
            ExecuteTaskUrl = address + "/api/Task/execute";
            AbortTaskUrl = address + "/api/Task/abort";
            Status = NodeStatus.Available;
            TaskAllocation = new List<TaskAllocation>();
        }
    }
}
