using Microsoft.EntityFrameworkCore;
using ServerMonitor.Domain.Entities;
using ServerMonitor.Infrastructure.Data;
using ServerMonitor.UseCases.Interfaces;

namespace ServerMonitor.Infrastructure.Repositories;

public class AlertRepository(IDbContextFactory<AppDbContext> contextFactory) : IAlertRepository
{
    public async Task<Alert?> GetOpenForTargetAsync(int targetId, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.Alerts
            .AsNoTracking()
            .Where(a => a.TargetId == targetId && a.ResolvedAt == null)
            .OrderByDescending(a => a.RaisedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Alert>> ListForOwnerAsync(string ownerId, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.Alerts
            .AsNoTracking()
            .Include(a => a.Target)
            .Where(a => a.Target!.OwnerId == ownerId)
            .OrderByDescending(a => a.RaisedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Alert?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.Alerts.AsNoTracking().Include(a => a.Target).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task AddAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        db.Alerts.Add(alert);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        db.Alerts.Attach(alert);
        db.Entry(alert).State = EntityState.Modified;
        await db.SaveChangesAsync(cancellationToken);
    }
}
