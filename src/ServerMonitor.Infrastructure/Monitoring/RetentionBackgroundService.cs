using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServerMonitor.UseCases.Interfaces;

namespace ServerMonitor.Infrastructure.Monitoring;

public class RetentionBackgroundService(
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider,
    IOptions<MonitoringOptions> options,
    ILogger<RetentionBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(6), timeProvider);

        do
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var checkResults = scope.ServiceProvider.GetRequiredService<ICheckResultRepository>();
                var cutoff = timeProvider.GetUtcNow().AddDays(-options.Value.RetentionDays);

                var deleted = await checkResults.DeleteOlderThanAsync(cutoff, stoppingToken);
                if (deleted > 0)
                {
                    logger.LogInformation("Deleted {Count} check results older than {Cutoff}", deleted, cutoff);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Retention sweep failed");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
