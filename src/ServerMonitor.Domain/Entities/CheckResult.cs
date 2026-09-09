using ServerMonitor.Domain.Enums;

namespace ServerMonitor.Domain.Entities;

public class CheckResult
{
    public long Id { get; set; }

    public int TargetId { get; set; }

    public MonitoredTarget? Target { get; set; }

    public DateTimeOffset CheckedAt { get; set; }

    public MonitorStatus Status { get; set; }

    public int? ResponseTimeMs { get; set; }

    public string? Error { get; set; }
}
