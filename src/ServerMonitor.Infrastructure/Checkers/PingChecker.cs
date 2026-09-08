using System.Net.NetworkInformation;
using ServerMonitor.Domain.Entities;
using ServerMonitor.Domain.Enums;
using ServerMonitor.UseCases.Interfaces;
using ServerMonitor.UseCases.Models;

namespace ServerMonitor.Infrastructure.Checkers;

public class PingChecker : IStatusChecker
{
    public TargetKind Kind => TargetKind.Ping;

    public async Task<CheckOutcome> CheckAsync(MonitoredTarget target, CancellationToken cancellationToken = default)
    {
        using var ping = new Ping();
        try
        {
            var reply = await ping.SendPingAsync(target.Address, TimeSpan.FromSeconds(target.TimeoutSeconds), cancellationToken: cancellationToken);

            return reply.Status == IPStatus.Success
                ? CheckOutcome.Up((int)reply.RoundtripTime)
                : CheckOutcome.Down(reply.Status.ToString());
        }
        catch (PingException ex)
        {
            return CheckOutcome.Down(ex.InnerException?.Message ?? ex.Message);
        }
    }
}
