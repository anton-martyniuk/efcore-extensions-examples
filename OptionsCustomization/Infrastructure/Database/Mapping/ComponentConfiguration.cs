using Domain.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mapping;

public class ComponentConfiguration : IEntityTypeConfiguration<Component>
{
    public void Configure(EntityTypeBuilder<Component> builder)
    {
        builder.HasKey(c => c.ComponentId);

        builder.Property(c => c.ComponentId).ValueGeneratedOnAdd();

        builder.Property(c => c.DeviceId).IsRequired();
        builder.Property(c => c.ComponentType).IsRequired();
        builder.Property(c => c.Name).IsRequired().HasMaxLength(250);
        builder.Property(c => c.Capability).IsRequired().HasMaxLength(250);
        builder.Property(c => c.Unit).IsRequired().HasMaxLength(50);
        builder.Property(c => c.StateValue).HasMaxLength(500);
        builder.Property(c => c.State).IsRequired();
        builder.Property(c => c.IsActive).IsRequired();
        builder.Property(c => c.LastUpdatedAt).IsRequired();

        builder.HasOne<Device>()
            .WithMany(x => x.Components)
            .HasForeignKey(c => c.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
