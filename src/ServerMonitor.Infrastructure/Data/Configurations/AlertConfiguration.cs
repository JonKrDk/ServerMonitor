using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServerMonitor.Domain.Entities;

namespace ServerMonitor.Infrastructure.Data.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.Property(a => a.Message).IsRequired().HasMaxLength(1000);

        builder.Ignore(a => a.IsResolved);

        builder.HasIndex(a => new { a.TargetId, a.ResolvedAt });
    }
}
