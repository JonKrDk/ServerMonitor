using Microsoft.EntityFrameworkCore;
using ServerMonitor.Domain.Entities;
using ServerMonitor.Infrastructure.Data;
using ServerMonitor.UseCases.Interfaces;

namespace ServerMonitor.Infrastructure.Repositories;

public class CheckResultRepository(AppDbContext db) : ICheckResultRepository
{
    public async Task AddAsync(CheckResult result, CancellationToken cancellationToken = default)
    {
        db.CheckResults.Add(result);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CheckResult>> ListForTargetAsync(int targetId, DateTimeOffset since, CancellationToken cancellationToken = default) =>
        await db.CheckResults
            .Where(r => r.TargetId == targetId && r.CheckedAt >= since)
            .OrderBy(r => r.CheckedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CheckResult>> ListRecentForTargetAsync(int targetId, int count, CancellationToken cancellationToken = default) =>
        await db.CheckResults
            .Where(r => r.TargetId == targetId)
            .OrderByDescending(r => r.CheckedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

    public Task<int> DeleteOlderThanAsync(DateTimeOffset cutoff, CancellationToken cancellationToken = default) =>
        db.CheckResults.Where(r => r.CheckedAt < cutoff).ExecuteDeleteAsync(cancellationToken);
}
