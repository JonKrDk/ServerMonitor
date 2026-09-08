using Microsoft.EntityFrameworkCore;
using ServerMonitor.Domain.Entities;
using ServerMonitor.Infrastructure.Data;
using ServerMonitor.UseCases.Interfaces;

namespace ServerMonitor.Infrastructure.Repositories;

public class CheckResultRepository(IDbContextFactory<AppDbContext> contextFactory) : ICheckResultRepository
{
    public async Task AddAsync(CheckResult result, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        db.CheckResults.Add(result);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CheckResult>> ListForTargetAsync(int targetId, DateTimeOffset since, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.CheckResults
            .AsNoTracking()
            .Where(r => r.TargetId == targetId && r.CheckedAt >= since)
            .OrderBy(r => r.CheckedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CheckResult>> ListRecentForTargetAsync(int targetId, int count, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.CheckResults
            .AsNoTracking()
            .Where(r => r.TargetId == targetId)
            .OrderByDescending(r => r.CheckedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> DeleteOlderThanAsync(DateTimeOffset cutoff, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.CheckResults.Where(r => r.CheckedAt < cutoff).ExecuteDeleteAsync(cancellationToken);
    }
}
