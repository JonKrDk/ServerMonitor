using ServerMonitor.Domain.Enums;

namespace ServerMonitor.UseCases.Models;

public record CheckOutcome(MonitorStatus Status, int? ResponseTimeMs, string? Error)
{
    public static CheckOutcome Up(int responseTimeMs) => new(MonitorStatus.Up, responseTimeMs, null);

    public static CheckOutcome Down(string error, int? responseTimeMs = null) => new(MonitorStatus.Down, responseTimeMs, error);
}
