using ServerMonitor.Domain.Entities;
using ServerMonitor.Domain.Enums;
using ServerMonitor.UseCases.Services;

namespace ServerMonitor.UnitTests;

public class StatisticsServiceTests
{
    private static readonly DateTimeOffset Origin = DateTimeOffset.Parse("2026-01-01T00:00:00Z");

    private static CheckResult Check(int minute, MonitorStatus status, int? responseTime) => new()
    {
        TargetId = 1,
        CheckedAt = Origin.AddMinutes(minute),
        Status = status,
        ResponseTimeMs = responseTime
    };

    [Fact]
    public void Summarize_returns_empty_for_no_checks()
    {
        Assert.Equal(0, StatisticsService.Summarize([]).TotalChecks);
        Assert.Null(StatisticsService.Summarize([]).AverageResponseTimeMs);
    }

    [Fact]
    public void Summarize_computes_uptime_error_rate_and_average_response_time()
    {
        CheckResult[] checks =
        [
            Check(0, MonitorStatus.Up, 100),
            Check(1, MonitorStatus.Up, 200),
            Check(2, MonitorStatus.Down, null),
            Check(3, MonitorStatus.Up, 300)
        ];

        var stats = StatisticsService.Summarize(checks);

        Assert.Equal(4, stats.TotalChecks);
        Assert.Equal(1, stats.FailedChecks);
        Assert.Equal(75, stats.UptimePercent);
        Assert.Equal(25, stats.ErrorRatePercent);
        Assert.Equal(200, stats.AverageResponseTimeMs);
    }

    [Fact]
    public void Bucket_groups_checks_into_fixed_windows()
    {
        CheckResult[] checks =
        [
            Check(0, MonitorStatus.Up, 100),
            Check(5, MonitorStatus.Down, null),
            Check(10, MonitorStatus.Up, 300)
        ];

        var buckets = StatisticsService.Bucket(checks, Origin, TimeSpan.FromMinutes(10));

        Assert.Equal(2, buckets.Count);
        Assert.Equal(Origin, buckets[0].Start);
        Assert.Equal(2, buckets[0].TotalChecks);
        Assert.Equal(50, buckets[0].UptimePercent);
        Assert.Equal(100, buckets[1].UptimePercent);
    }
}
