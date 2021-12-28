using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IHostedServiceDemo;

public class HostedService : IHostedService, IAsyncDisposable
{
    private readonly ILogger<HostedService> _logger;
    private int _executionCount;
    private Timer? _timer;

    public HostedService(ILogger<HostedService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _executionCount = 0;
        _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1));

        _logger.LogInformation($"Started {nameof(HostedService)}...");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Stopping {nameof(HostedService)}...");

        _timer?.Change(Timeout.Infinite, 0);

        _logger.LogInformation($"Stopped {nameof(HostedService)}.");

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_timer is IAsyncDisposable timer)
        {
            await timer.DisposeAsync();
        }

        _timer = null;
    }

    private void DoWork(object? state)
    {
        var count = Interlocked.Increment(ref _executionCount);

        _logger.LogInformation("{Count}", count);
    }
}