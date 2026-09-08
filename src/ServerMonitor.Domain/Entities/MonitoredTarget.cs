using ServerMonitor.Domain.Enums;

namespace ServerMonitor.Domain.Entities;

public class MonitoredTarget
{
    public int Id { get; set; }

    public string OwnerId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public TargetKind Kind { get; set; }

    /// <summary>URL for HTTP targets, host name or IP address for Ping and TCP targets.</summary>
    public string Address { get; set; } = string.Empty;

    public int? Port { get; set; }

    public int? ExpectedStatusCode { get; set; }

    public int CheckIntervalSeconds { get; set; } = 60;

    public int TimeoutSeconds { get; set; } = 10;

    public bool IsEnabled { get; set; } = true;

    public MonitorStatus Status { get; set; } = MonitorStatus.Unknown;

    public DateTimeOffset? LastCheckedAt { get; set; }

    public int ConsecutiveFailures { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<CheckResult> CheckResults { get; set; } = [];

    public ICollection<Alert> Alerts { get; set; } = [];

    public bool IsDue(DateTimeOffset now) =>
        IsEnabled && (LastCheckedAt is null || now - LastCheckedAt >= TimeSpan.FromSeconds(CheckIntervalSeconds));
}
