using Domain.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mapping;

public class TelemetryConfiguration : IEntityTypeConfiguration<Telemetry>
{
    public void Configure(EntityTypeBuilder<Telemetry> builder)
    {
        builder.HasKey(t => t.TelemetryId);

        builder.Property(t => t.TelemetryId).ValueGeneratedOnAdd();

        builder.Property(t => t.DeviceId).IsRequired();
        builder.Property(t => t.ComponentId).IsRequired();
        builder.Property(t => t.Value).IsRequired();
        builder.Property(t => t.Quality).IsRequired();
        builder.Property(t => t.CollectedAt).IsRequired();
        builder.Property(t => t.ReceivedAt).IsRequired();

        builder.HasOne<Device>()
            .WithMany()
            .HasForeignKey(t => t.DeviceId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Component>()
            .WithMany()
            .HasForeignKey(t => t.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
