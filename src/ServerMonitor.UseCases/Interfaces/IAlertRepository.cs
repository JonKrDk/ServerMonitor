using ServerMonitor.Domain.Entities;

namespace ServerMonitor.UseCases.Interfaces;

public interface IAlertRepository
{
    Task<Alert?> GetOpenForTargetAsync(int targetId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Alert>> ListForOwnerAsync(string ownerId, CancellationToken cancellationToken = default);

    Task<Alert?> GetAsync(int id, CancellationToken cancellationToken = default);

    Task AddAsync(Alert alert, CancellationToken cancellationToken = default);

    Task UpdateAsync(Alert alert, CancellationToken cancellationToken = default);
}
