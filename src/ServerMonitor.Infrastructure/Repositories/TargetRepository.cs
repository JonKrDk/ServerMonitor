using Microsoft.EntityFrameworkCore;
using ServerMonitor.Domain.Entities;
using ServerMonitor.Infrastructure.Data;
using ServerMonitor.UseCases.Interfaces;

namespace ServerMonitor.Infrastructure.Repositories;

public class TargetRepository(IDbContextFactory<AppDbContext> contextFactory) : ITargetRepository
{
    public async Task<IReadOnlyList<MonitoredTarget>> ListForOwnerAsync(string ownerId, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.Targets.AsNoTracking().Where(t => t.OwnerId == ownerId).OrderBy(t => t.Name).ToListAsync(cancellationToken);
    }

    public async Task<MonitoredTarget?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.Targets.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<MonitoredTarget>> ListDueAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.Targets
            .AsNoTracking()
            .Where(t => t.IsEnabled &&
                (t.LastCheckedAt == null ||
                 EF.Functions.DateDiffSecond(t.LastCheckedAt.Value, now) >= t.CheckIntervalSeconds))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MonitoredTarget target, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        db.Targets.Add(target);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MonitoredTarget target, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        db.Targets.Attach(target);
        db.Entry(target).State = EntityState.Modified;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MonitoredTarget target, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        db.Targets.Remove(target);
        await db.SaveChangesAsync(cancellationToken);
    }
}
