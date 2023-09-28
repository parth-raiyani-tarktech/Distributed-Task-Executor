namespace TaskExecutor.Models
{
    public class Node
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Url { get; set; }
        public NodeStatus Status { get; set; }

        public List<TaskAllocation> TaskAllocation { get; set; }

        public Node(string name, string address)
        {
            Id = Guid.NewGuid();
            Name = name;
            Address = address;
            Url = address + "/api/Task/execute";
            Status = NodeStatus.Available;
            TaskAllocation = new List<TaskAllocation>();
        }
    }
}
