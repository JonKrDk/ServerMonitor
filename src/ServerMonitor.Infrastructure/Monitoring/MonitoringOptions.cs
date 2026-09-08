namespace ServerMonitor.Infrastructure.Monitoring;

public class MonitoringOptions
{
    public const string SectionName = "Monitoring";

    public int PollIntervalSeconds { get; set; } = 5;

    public int MaxConcurrentChecks { get; set; } = 10;

    public int RetentionDays { get; set; } = 30;
}
