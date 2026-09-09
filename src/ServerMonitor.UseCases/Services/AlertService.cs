using ServerMonitor.Domain.Entities;
using ServerMonitor.UseCases.Interfaces;

namespace ServerMonitor.UseCases.Services;

public class AlertService(IAlertRepository alerts)
{
    public Task<IReadOnlyList<Alert>> ListAsync(string ownerId, CancellationToken cancellationToken = default) =>
        alerts.ListForOwnerAsync(ownerId, cancellationToken);

    public async Task<bool> AcknowledgeAsync(int alertId, string ownerId, CancellationToken cancellationToken = default)
    {
        var alert = await alerts.GetAsync(alertId, cancellationToken);
        if (alert?.Target?.OwnerId != ownerId)
        {
            return false;
        }

        alert.IsAcknowledged = true;
        await alerts.UpdateAsync(alert, cancellationToken);
        return true;
    }
}
