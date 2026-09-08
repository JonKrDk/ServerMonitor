using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServerMonitor.Domain.Entities;
using ServerMonitor.UseCases.Interfaces;
using ServerMonitor.UseCases.Services;

namespace ServerMonitor.Infrastructure.Monitoring;

public class MonitoringBackgroundService(
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider,
    IOptions<MonitoringOptions> options,
    ILogger<MonitoringBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(options.Value.PollIntervalSeconds), timeProvider);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await RunDueChecksAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Monitoring sweep failed");
            }
        }
    }

    private async Task RunDueChecksAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<MonitoredTarget> due;
        using (var scope = scopeFactory.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<ITargetRepository>();
            due = await repository.ListDueAsync(timeProvider.GetUtcNow(), cancellationToken);
        }

        if (due.Count == 0)
        {
            return;
        }

        using var gate = new SemaphoreSlim(options.Value.MaxConcurrentChecks);
        await Task.WhenAll(due.Select(target => CheckAsync(target, gate, cancellationToken)));
    }

    private async Task CheckAsync(MonitoredTarget target, SemaphoreSlim gate, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            using var scope = scopeFactory.CreateScope();
            var monitoring = scope.ServiceProvider.GetRequiredService<MonitoringService>();
            await monitoring.CheckAsync(target, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Check failed for target {TargetId}", target.Id);
        }
        finally
        {
            gate.Release();
        }
    }
}
