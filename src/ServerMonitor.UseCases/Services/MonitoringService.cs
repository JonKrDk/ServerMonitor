using ServerMonitor.Domain.Entities;
using ServerMonitor.Domain.Enums;
using ServerMonitor.UseCases.Interfaces;
using ServerMonitor.UseCases.Models;

namespace ServerMonitor.UseCases.Services;

public class MonitoringService(
    IEnumerable<IStatusChecker> checkers,
    ITargetRepository targets,
    ICheckResultRepository checkResults,
    IAlertRepository alerts,
    IMonitorNotifier notifier,
    TimeProvider timeProvider)
{
    public async Task<CheckResult> CheckAsync(MonitoredTarget target, CancellationToken cancellationToken = default)
    {
        var checker = checkers.First(c => c.Kind == target.Kind);
        var outcome = await checker.CheckAsync(target, cancellationToken);
        var now = timeProvider.GetUtcNow();

        var result = new CheckResult
        {
            TargetId = target.Id,
            CheckedAt = now,
            Status = outcome.Status,
            ResponseTimeMs = outcome.ResponseTimeMs,
            Error = Truncate(outcome.Error)
        };
        await checkResults.AddAsync(result, cancellationToken);

        var previousStatus = target.Status;
        target.Status = outcome.Status;
        target.LastCheckedAt = now;
        target.ConsecutiveFailures = outcome.Status == MonitorStatus.Down ? target.ConsecutiveFailures + 1 : 0;
        await targets.UpdateAsync(target, cancellationToken);

        if (outcome.Status == MonitorStatus.Down && previousStatus != MonitorStatus.Down)
        {
            var alert = new Alert
            {
                TargetId = target.Id,
                RaisedAt = now,
                Message = $"{target.Name} is down: {outcome.Error ?? "check failed"}"
            };
            await alerts.AddAsync(alert, cancellationToken);
            await notifier.AlertRaisedAsync(target.OwnerId, alert, cancellationToken);
        }
        else if (outcome.Status == MonitorStatus.Up && previousStatus == MonitorStatus.Down)
        {
            var open = await alerts.GetOpenForTargetAsync(target.Id, cancellationToken);
            if (open is not null)
            {
                open.ResolvedAt = now;
                await alerts.UpdateAsync(open, cancellationToken);
            }
        }

        await notifier.StatusUpdatedAsync(
            target.OwnerId,
            new TargetStatusUpdate(target.Id, target.Name, result.Status, result.CheckedAt, result.ResponseTimeMs, result.Error),
            cancellationToken);

        return result;
    }

    private static string? Truncate(string? error) =>
        error is { Length: > 1000 } ? error[..1000] : error;
}
