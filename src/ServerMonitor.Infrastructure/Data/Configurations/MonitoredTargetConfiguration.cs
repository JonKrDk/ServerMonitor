using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServerMonitor.Domain.Entities;

namespace ServerMonitor.Infrastructure.Data.Configurations;

public class MonitoredTargetConfiguration : IEntityTypeConfiguration<MonitoredTarget>
{
    public void Configure(EntityTypeBuilder<MonitoredTarget> builder)
    {
        builder.Property(t => t.OwnerId).IsRequired().HasMaxLength(450);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Address).IsRequired().HasMaxLength(2048);

        builder.HasIndex(t => t.OwnerId);

        builder.HasMany(t => t.CheckResults)
            .WithOne(r => r.Target!)
            .HasForeignKey(r => r.TargetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Alerts)
            .WithOne(a => a.Target!)
            .HasForeignKey(a => a.TargetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
