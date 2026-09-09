using ServerMonitor.Domain.Entities;
using ServerMonitor.Domain.Enums;
using ServerMonitor.UseCases.Models;

namespace ServerMonitor.UseCases.Interfaces;

public interface IStatusChecker
{
    TargetKind Kind { get; }

    Task<CheckOutcome> CheckAsync(MonitoredTarget target, CancellationToken cancellationToken = default);
}
