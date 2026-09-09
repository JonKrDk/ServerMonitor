using ServerMonitor.Domain.Entities;
using ServerMonitor.UseCases.Models;

namespace ServerMonitor.UseCases.Interfaces;

public interface IMonitorNotifier
{
    Task StatusUpdatedAsync(string ownerId, TargetStatusUpdate update, CancellationToken cancellationToken = default);

    Task AlertRaisedAsync(string ownerId, Alert alert, CancellationToken cancellationToken = default);
}
