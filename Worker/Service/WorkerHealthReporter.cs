using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading;

namespace Worker.Service
{
    public class WorkerHealthReporter : BackgroundService
    {
        private readonly WorkerInfo _worker;
        private readonly string _allocatorUri;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public WorkerHealthReporter(
            IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration, WorkerInfo worker)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _worker = worker;
            _allocatorUri = configuration.GetValue<string>("AllocatorUri");
            _hostApplicationLifetime.ApplicationStopping.Register(() => OnStoppedAsync());
        }


        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(async() => await OnStoppedAsync());
            while (!stoppingToken.IsCancellationRequested)
            {
                await OnStoppedAsync();
                await Task.Delay(TimeSpan.FromSeconds(120));
            }
        }

        private async Task OnStoppedAsync()
        {
            var client = new HttpClient();
            await client.PostAsJsonAsync($"{_allocatorUri}/api/nodes/shutting-down", new
            {
                _worker?.Name
            });
        }
    }
}
