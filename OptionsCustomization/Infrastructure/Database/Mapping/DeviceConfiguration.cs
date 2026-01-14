using Domain.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mapping;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasKey(d => d.DeviceId);

        builder.Property(d => d.DeviceId).ValueGeneratedOnAdd();

        builder.Property(d => d.Name).IsRequired().HasMaxLength(250);
        builder.Property(d => d.DeviceType).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Manufacturer).IsRequired().HasMaxLength(250);
        builder.Property(d => d.SerialNumber).IsRequired().HasMaxLength(100);
        builder.Property(d => d.FirmwareVersion).IsRequired().HasMaxLength(50);
        builder.Property(d => d.HardwareVersion).IsRequired().HasMaxLength(50);
        builder.Property(d => d.Status).IsRequired();
        builder.Property(d => d.LastSeenAt).IsRequired();
        builder.Property(d => d.RegisteredAt).IsRequired();
        builder.Property(d => d.Configuration).HasMaxLength(4000);
    }
}
