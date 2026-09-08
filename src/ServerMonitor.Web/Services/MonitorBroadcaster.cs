using ServerMonitor.Domain.Entities;
using ServerMonitor.UseCases.Interfaces;
using ServerMonitor.UseCases.Models;

namespace ServerMonitor.Web.Services;

/// <summary>
/// Fans check results out to the Blazor circuits of the owning user, which deliver them
/// to the browser over the circuit's SignalR connection.
/// </summary>
public sealed class MonitorBroadcaster : IMonitorNotifier
{
    public event Action<string, TargetStatusUpdate>? StatusUpdated;

    public event Action<string, Alert>? AlertRaised;

    public Task StatusUpdatedAsync(string ownerId, TargetStatusUpdate update, CancellationToken cancellationToken = default)
    {
        StatusUpdated?.Invoke(ownerId, update);
        return Task.CompletedTask;
    }

    public Task AlertRaisedAsync(string ownerId, Alert alert, CancellationToken cancellationToken = default)
    {
        AlertRaised?.Invoke(ownerId, alert);
        return Task.CompletedTask;
    }
}
