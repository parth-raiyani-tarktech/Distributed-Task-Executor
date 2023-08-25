namespace Worker;

public class WorkerInfo
{
    public WorkerInfo(string name, int port)
    {
        Name = name;
        Port = port;
        WorkDir = Path.Combine(Environment.CurrentDirectory, "work");
    }
    
    public string Name { get; set; }
    public int Port { get; set; }
    public string WorkDir { get; set; }
}