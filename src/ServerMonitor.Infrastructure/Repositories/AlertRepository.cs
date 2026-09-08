using Microsoft.EntityFrameworkCore;
using ServerMonitor.Domain.Entities;
using ServerMonitor.Infrastructure.Data;
using ServerMonitor.UseCases.Interfaces;

namespace ServerMonitor.Infrastructure.Repositories;

public class AlertRepository(AppDbContext db) : IAlertRepository
{
    public Task<Alert?> GetOpenForTargetAsync(int targetId, CancellationToken cancellationToken = default) =>
        db.Alerts
            .Where(a => a.TargetId == targetId && a.ResolvedAt == null)
            .OrderByDescending(a => a.RaisedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<Alert>> ListForOwnerAsync(string ownerId, CancellationToken cancellationToken = default) =>
        await db.Alerts
            .Include(a => a.Target)
            .Where(a => a.Target!.OwnerId == ownerId)
            .OrderByDescending(a => a.RaisedAt)
            .ToListAsync(cancellationToken);

    public Task<Alert?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        db.Alerts.Include(a => a.Target).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task AddAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        db.Alerts.Add(alert);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        db.Alerts.Update(alert);
        await db.SaveChangesAsync(cancellationToken);
    }
}
