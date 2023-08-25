namespace Worker;

public class WorkerRegistrar
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<WorkerRegistrar> _logger;
    private readonly string _allocatorUri;

    public WorkerRegistrar(IConfiguration configuration, ILogger<WorkerRegistrar> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _allocatorUri = configuration.GetValue<string>("AllocatorUri");
    }

    public WorkerInfo GetWorkerInfo()
    {
        return new WorkerInfo(
            _configuration.GetValue<string>("name"),
            _configuration.GetValue<int>("port")
        );
    }
    
    public async Task RegisterWorkerAsync()
    {
        var worker = GetWorkerInfo();
        
        var client = new HttpClient();
        var response = await client.PostAsJsonAsync($"{_allocatorUri}/api/nodes/register", new
        {
            worker.Name,
            Address = $"http://localhost:{worker.Port}"
        });
        response.EnsureSuccessStatusCode();
        
        _logger.LogInformation("Worker registered with name: {WorkerName}", worker.Name);
    }
}