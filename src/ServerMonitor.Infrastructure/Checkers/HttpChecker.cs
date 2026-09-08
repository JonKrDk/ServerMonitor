using System.Diagnostics;
using ServerMonitor.Domain.Entities;
using ServerMonitor.Domain.Enums;
using ServerMonitor.UseCases.Interfaces;
using ServerMonitor.UseCases.Models;

namespace ServerMonitor.Infrastructure.Checkers;

public class HttpChecker(IHttpClientFactory httpClientFactory) : IStatusChecker
{
    public TargetKind Kind => TargetKind.Http;

    public async Task<CheckOutcome> CheckAsync(MonitoredTarget target, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(nameof(HttpChecker));
        client.Timeout = TimeSpan.FromSeconds(target.TimeoutSeconds);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var response = await client.GetAsync(target.Address, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            stopwatch.Stop();

            var elapsed = (int)stopwatch.ElapsedMilliseconds;
            var expected = target.ExpectedStatusCode;

            if (expected.HasValue)
            {
                return (int)response.StatusCode == expected.Value
                    ? CheckOutcome.Up(elapsed)
                    : CheckOutcome.Down($"Expected status {expected.Value} but got {(int)response.StatusCode}", elapsed);
            }

            return response.IsSuccessStatusCode
                ? CheckOutcome.Up(elapsed)
                : CheckOutcome.Down($"HTTP {(int)response.StatusCode} {response.ReasonPhrase}", elapsed);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return CheckOutcome.Down($"Timed out after {target.TimeoutSeconds}s");
        }
        catch (HttpRequestException ex)
        {
            return CheckOutcome.Down(ex.Message);
        }
    }
}
