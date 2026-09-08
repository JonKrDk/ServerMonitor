using ServerMonitor.Domain.Entities;
using ServerMonitor.Domain.Enums;
using ServerMonitor.UseCases.Interfaces;
using ServerMonitor.UseCases.Models;

namespace ServerMonitor.UseCases.Services;

public class StatisticsService(ICheckResultRepository checkResults)
{
    public async Task<TargetStatistics> GetStatisticsAsync(int targetId, DateTimeOffset since, CancellationToken cancellationToken = default) =>
        Summarize(await checkResults.ListForTargetAsync(targetId, since, cancellationToken));

    public async Task<IReadOnlyList<StatisticsBucket>> GetSeriesAsync(
        int targetId,
        DateTimeOffset since,
        TimeSpan bucketSize,
        CancellationToken cancellationToken = default) =>
        Bucket(await checkResults.ListForTargetAsync(targetId, since, cancellationToken), since, bucketSize);

    public static TargetStatistics Summarize(IReadOnlyCollection<CheckResult> checks)
    {
        if (checks.Count == 0)
        {
            return TargetStatistics.Empty;
        }

        var failed = checks.Count(c => c.Status != MonitorStatus.Up);
        var uptime = (double)(checks.Count - failed) / checks.Count * 100;
        var responseTimes = checks.Where(c => c.ResponseTimeMs.HasValue).Select(c => (double)c.ResponseTimeMs!.Value).ToList();

        return new TargetStatistics(checks.Count, failed, uptime, responseTimes.Count > 0 ? responseTimes.Average() : null);
    }

    public static IReadOnlyList<StatisticsBucket> Bucket(IEnumerable<CheckResult> checks, DateTimeOffset since, TimeSpan bucketSize)
    {
        return checks
            .GroupBy(c => since + TimeSpan.FromTicks((c.CheckedAt - since).Ticks / bucketSize.Ticks * bucketSize.Ticks))
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var summary = Summarize(g.ToList());
                return new StatisticsBucket(g.Key, summary.TotalChecks, summary.UptimePercent, summary.AverageResponseTimeMs);
            })
            .ToList();
    }
}
