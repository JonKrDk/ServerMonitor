namespace ServerMonitor.Domain.Entities;

public class Alert
{
    public int Id { get; set; }

    public int TargetId { get; set; }

    public MonitoredTarget? Target { get; set; }

    public DateTimeOffset RaisedAt { get; set; }

    public DateTimeOffset? ResolvedAt { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool IsAcknowledged { get; set; }

    public bool IsResolved => ResolvedAt.HasValue;
}
