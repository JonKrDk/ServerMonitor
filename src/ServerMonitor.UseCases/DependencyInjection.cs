using Microsoft.Extensions.DependencyInjection;
using ServerMonitor.UseCases.Services;

namespace ServerMonitor.UseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<TargetService>();
        services.AddScoped<MonitoringService>();
        services.AddScoped<StatisticsService>();
        services.AddScoped<AlertService>();

        return services;
    }
}
