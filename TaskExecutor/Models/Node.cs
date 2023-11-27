namespace TaskExecutor.Models
{
    public class Node
    {
        // REVIEW:
        //  These are good set of properties to track on a node. But we should also be mindful about the access level that we expose to the class's consumer.
        //  Most of these props are public and Read-Write. While ideally, a node's name/id/address should not be changed once created. Would be better to make those props read-only

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
