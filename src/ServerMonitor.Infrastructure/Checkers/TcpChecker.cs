using System.Diagnostics;
using System.Net.Sockets;
using ServerMonitor.Domain.Entities;
using ServerMonitor.Domain.Enums;
using ServerMonitor.UseCases.Interfaces;
using ServerMonitor.UseCases.Models;

namespace ServerMonitor.Infrastructure.Checkers;

public class TcpChecker : IStatusChecker
{
    public TargetKind Kind => TargetKind.Tcp;

    public async Task<CheckOutcome> CheckAsync(MonitoredTarget target, CancellationToken cancellationToken = default)
    {
        if (target.Port is not { } port)
        {
            return CheckOutcome.Down("No port configured");
        }

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(target.TimeoutSeconds));

        using var client = new TcpClient();
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await client.ConnectAsync(target.Address, port, timeout.Token);
            stopwatch.Stop();
            return CheckOutcome.Up((int)stopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return CheckOutcome.Down($"Timed out after {target.TimeoutSeconds}s");
        }
        catch (SocketException ex)
        {
            return CheckOutcome.Down(ex.Message);
        }
    }
}
