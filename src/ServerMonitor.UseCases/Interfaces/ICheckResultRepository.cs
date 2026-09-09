using ServerMonitor.Domain.Entities;

namespace ServerMonitor.UseCases.Interfaces;

public interface ICheckResultRepository
{
    Task AddAsync(CheckResult result, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CheckResult>> ListForTargetAsync(int targetId, DateTimeOffset since, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CheckResult>> ListRecentForTargetAsync(int targetId, int count, CancellationToken cancellationToken = default);

    Task<int> DeleteOlderThanAsync(DateTimeOffset cutoff, CancellationToken cancellationToken = default);
}
