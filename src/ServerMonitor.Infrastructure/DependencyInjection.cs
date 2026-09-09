using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerMonitor.Infrastructure.Checkers;
using ServerMonitor.Infrastructure.Data;
using ServerMonitor.Infrastructure.Monitoring;
using ServerMonitor.Infrastructure.Repositories;
using ServerMonitor.UseCases.Interfaces;

namespace ServerMonitor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Blazor renders components concurrently within one circuit, so each operation gets its
        // own context from the factory; Identity still resolves a scoped one.
        services.AddDbContextFactory<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<AppDbContext>(sp => sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

        services.AddScoped<ITargetRepository, TargetRepository>();
        services.AddScoped<ICheckResultRepository, CheckResultRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();

        services.AddHttpClient(nameof(HttpChecker));
        services.AddScoped<IStatusChecker, HttpChecker>();
        services.AddScoped<IStatusChecker, PingChecker>();
        services.AddScoped<IStatusChecker, TcpChecker>();

        services.Configure<MonitoringOptions>(configuration.GetSection(MonitoringOptions.SectionName));
        services.AddHostedService<MonitoringBackgroundService>();
        services.AddHostedService<RetentionBackgroundService>();

        return services;
    }
}
