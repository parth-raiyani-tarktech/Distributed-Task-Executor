namespace TaskExecutor.Models
{
    public class Node
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public NodeStatus Status { get; set; }

        public Node(string name, string address)
        {
            this.Name = name;
            this.Address = address;
            this.Status = NodeStatus.Available;
        }
    }
}
