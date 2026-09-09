using ServerMonitor.Domain.Entities;
using ServerMonitor.Domain.Enums;
using ServerMonitor.UseCases.Interfaces;
using ServerMonitor.UseCases.Models;

namespace ServerMonitor.UnitTests;

public class FakeTargetRepository : ITargetRepository
{
    public List<MonitoredTarget> Targets { get; } = [];

    public Task<IReadOnlyList<MonitoredTarget>> ListForOwnerAsync(string ownerId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<MonitoredTarget>>(Targets.Where(t => t.OwnerId == ownerId).ToList());

    public Task<MonitoredTarget?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Targets.FirstOrDefault(t => t.Id == id));

    public Task<IReadOnlyList<MonitoredTarget>> ListDueAsync(DateTimeOffset now, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<MonitoredTarget>>(Targets.Where(t => t.IsDue(now)).ToList());

    public Task AddAsync(MonitoredTarget target, CancellationToken cancellationToken = default)
    {
        Targets.Add(target);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(MonitoredTarget target, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task DeleteAsync(MonitoredTarget target, CancellationToken cancellationToken = default)
    {
        Targets.Remove(target);
        return Task.CompletedTask;
    }
}

public class FakeCheckResultRepository : ICheckResultRepository
{
    public List<CheckResult> Results { get; } = [];

    public Task AddAsync(CheckResult result, CancellationToken cancellationToken = default)
    {
        Results.Add(result);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<CheckResult>> ListForTargetAsync(int targetId, DateTimeOffset since, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CheckResult>>(Results.Where(r => r.TargetId == targetId && r.CheckedAt >= since).ToList());

    public Task<IReadOnlyList<CheckResult>> ListRecentForTargetAsync(int targetId, int count, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CheckResult>>(Results.Where(r => r.TargetId == targetId).TakeLast(count).ToList());

    public Task<int> DeleteOlderThanAsync(DateTimeOffset cutoff, CancellationToken cancellationToken = default) =>
        Task.FromResult(Results.RemoveAll(r => r.CheckedAt < cutoff));
}

public class FakeAlertRepository : IAlertRepository
{
    public List<Alert> Alerts { get; } = [];

    public Task<Alert?> GetOpenForTargetAsync(int targetId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Alerts.FirstOrDefault(a => a.TargetId == targetId && a.ResolvedAt == null));

    public Task<IReadOnlyList<Alert>> ListForOwnerAsync(string ownerId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Alert>>(Alerts.Where(a => a.Target?.OwnerId == ownerId).ToList());

    public Task<Alert?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Alerts.FirstOrDefault(a => a.Id == id));

    public Task AddAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        Alerts.Add(alert);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Alert alert, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public class FakeNotifier : IMonitorNotifier
{
    public List<TargetStatusUpdate> Updates { get; } = [];

    public List<Alert> RaisedAlerts { get; } = [];

    public Task StatusUpdatedAsync(string ownerId, TargetStatusUpdate update, CancellationToken cancellationToken = default)
    {
        Updates.Add(update);
        return Task.CompletedTask;
    }

    public Task AlertRaisedAsync(string ownerId, Alert alert, CancellationToken cancellationToken = default)
    {
        RaisedAlerts.Add(alert);
        return Task.CompletedTask;
    }
}

public class StubChecker(TargetKind kind) : IStatusChecker
{
    public TargetKind Kind { get; } = kind;

    public CheckOutcome NextOutcome { get; set; } = CheckOutcome.Up(1);

    public Task<CheckOutcome> CheckAsync(MonitoredTarget target, CancellationToken cancellationToken = default) =>
        Task.FromResult(NextOutcome);
}
