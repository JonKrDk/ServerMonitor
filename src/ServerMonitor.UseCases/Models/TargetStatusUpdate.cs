using ServerMonitor.Domain.Enums;

namespace ServerMonitor.UseCases.Models;

public record TargetStatusUpdate(
    int TargetId,
    string Name,
    MonitorStatus Status,
    DateTimeOffset CheckedAt,
    int? ResponseTimeMs,
    string? Error);
