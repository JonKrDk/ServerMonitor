using ServerMonitor.Domain.Entities;

namespace ServerMonitor.UseCases.Interfaces;

public interface ITargetRepository
{
    Task<IReadOnlyList<MonitoredTarget>> ListForOwnerAsync(string ownerId, CancellationToken cancellationToken = default);

    Task<MonitoredTarget?> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MonitoredTarget>> ListDueAsync(DateTimeOffset now, CancellationToken cancellationToken = default);

    Task AddAsync(MonitoredTarget target, CancellationToken cancellationToken = default);

    Task UpdateAsync(MonitoredTarget target, CancellationToken cancellationToken = default);

    Task DeleteAsync(MonitoredTarget target, CancellationToken cancellationToken = default);
}
