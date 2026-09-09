using Microsoft.Extensions.Time.Testing;
using ServerMonitor.Domain.Entities;
using ServerMonitor.Domain.Enums;
using ServerMonitor.UseCases.Models;
using ServerMonitor.UseCases.Services;

namespace ServerMonitor.UnitTests;

public class MonitoringServiceTests
{
    private readonly FakeTargetRepository _targets = new();
    private readonly FakeCheckResultRepository _results = new();
    private readonly FakeAlertRepository _alerts = new();
    private readonly FakeNotifier _notifier = new();
    private readonly StubChecker _checker = new(TargetKind.Http);
    private readonly FakeTimeProvider _time = new(DateTimeOffset.Parse("2026-01-01T00:00:00Z"));

    private MonitoringService CreateService() =>
        new([_checker], _targets, _results, _alerts, _notifier, _time);

    private static MonitoredTarget Target(MonitorStatus status = MonitorStatus.Unknown) => new()
    {
        Id = 1,
        OwnerId = "owner-1",
        Name = "API",
        Kind = TargetKind.Http,
        Address = "https://example.test",
        Status = status
    };

    [Fact]
    public async Task Failing_check_raises_alert_and_records_result()
    {
        var target = Target(MonitorStatus.Up);
        _checker.NextOutcome = CheckOutcome.Down("HTTP 500");

        await CreateService().CheckAsync(target);

        Assert.Equal(MonitorStatus.Down, target.Status);
        Assert.Equal(1, target.ConsecutiveFailures);
        Assert.Single(_results.Results);
        var alert = Assert.Single(_alerts.Alerts);
        Assert.Null(alert.ResolvedAt);
        Assert.Single(_notifier.RaisedAlerts);
        Assert.Single(_notifier.Updates);
    }

    [Fact]
    public async Task Repeated_failure_does_not_raise_a_second_alert()
    {
        var target = Target(MonitorStatus.Up);
        _checker.NextOutcome = CheckOutcome.Down("HTTP 500");
        var service = CreateService();

        await service.CheckAsync(target);
        await service.CheckAsync(target);

        Assert.Single(_alerts.Alerts);
        Assert.Equal(2, target.ConsecutiveFailures);
    }

    [Fact]
    public async Task Recovery_resolves_the_open_alert()
    {
        var target = Target(MonitorStatus.Up);
        var service = CreateService();

        _checker.NextOutcome = CheckOutcome.Down("HTTP 500");
        await service.CheckAsync(target);

        _time.Advance(TimeSpan.FromMinutes(5));
        _checker.NextOutcome = CheckOutcome.Up(42);
        await service.CheckAsync(target);

        var alert = Assert.Single(_alerts.Alerts);
        Assert.Equal(_time.GetUtcNow(), alert.ResolvedAt);
        Assert.Equal(MonitorStatus.Up, target.Status);
        Assert.Equal(0, target.ConsecutiveFailures);
    }

    [Fact]
    public async Task Successful_check_records_response_time()
    {
        var target = Target();
        _checker.NextOutcome = CheckOutcome.Up(123);

        var result = await CreateService().CheckAsync(target);

        Assert.Equal(MonitorStatus.Up, result.Status);
        Assert.Equal(123, result.ResponseTimeMs);
        Assert.Equal(_time.GetUtcNow(), target.LastCheckedAt);
        Assert.Empty(_alerts.Alerts);
    }
}
