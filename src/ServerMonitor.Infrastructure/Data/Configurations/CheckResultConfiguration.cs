using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServerMonitor.Domain.Entities;

namespace ServerMonitor.Infrastructure.Data.Configurations;

public class CheckResultConfiguration : IEntityTypeConfiguration<CheckResult>
{
    public void Configure(EntityTypeBuilder<CheckResult> builder)
    {
        builder.Property(r => r.Error).HasMaxLength(1000);

        builder.HasIndex(r => new { r.TargetId, r.CheckedAt });
    }
}
