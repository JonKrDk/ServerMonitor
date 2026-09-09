namespace ServerMonitor.UseCases.Models;

public record TargetStatistics(
    int TotalChecks,
    int FailedChecks,
    double UptimePercent,
    double? AverageResponseTimeMs)
{
    public double ErrorRatePercent => 100 - UptimePercent;

    public static TargetStatistics Empty { get; } = new(0, 0, 0, null);
}

public record StatisticsBucket(
    DateTimeOffset Start,
    int TotalChecks,
    double UptimePercent,
    double? AverageResponseTimeMs);
