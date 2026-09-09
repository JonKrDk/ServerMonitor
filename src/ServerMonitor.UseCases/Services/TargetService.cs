using ServerMonitor.Domain.Entities;
using ServerMonitor.UseCases.Interfaces;

namespace ServerMonitor.UseCases.Services;

public class TargetService(ITargetRepository targets, TimeProvider timeProvider)
{
    public Task<IReadOnlyList<MonitoredTarget>> ListAsync(string ownerId, CancellationToken cancellationToken = default) =>
        targets.ListForOwnerAsync(ownerId, cancellationToken);

    public async Task<MonitoredTarget?> GetAsync(int id, string ownerId, CancellationToken cancellationToken = default)
    {
        var target = await targets.GetAsync(id, cancellationToken);
        return target?.OwnerId == ownerId ? target : null;
    }

    public async Task<MonitoredTarget> CreateAsync(string ownerId, MonitoredTarget target, CancellationToken cancellationToken = default)
    {
        target.OwnerId = ownerId;
        target.CreatedAt = timeProvider.GetUtcNow();
        await targets.AddAsync(target, cancellationToken);
        return target;
    }

    public async Task<bool> UpdateAsync(string ownerId, MonitoredTarget edited, CancellationToken cancellationToken = default)
    {
        var target = await GetAsync(edited.Id, ownerId, cancellationToken);
        if (target is null)
        {
            return false;
        }

        target.Name = edited.Name;
        target.Kind = edited.Kind;
        target.Address = edited.Address;
        target.Port = edited.Port;
        target.ExpectedStatusCode = edited.ExpectedStatusCode;
        target.CheckIntervalSeconds = edited.CheckIntervalSeconds;
        target.TimeoutSeconds = edited.TimeoutSeconds;
        target.IsEnabled = edited.IsEnabled;

        await targets.UpdateAsync(target, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string ownerId, CancellationToken cancellationToken = default)
    {
        var target = await GetAsync(id, ownerId, cancellationToken);
        if (target is null)
        {
            return false;
        }

        await targets.DeleteAsync(target, cancellationToken);
        return true;
    }
}
