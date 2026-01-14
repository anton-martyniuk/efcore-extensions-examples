using Domain.Devices;
using Infrastructure.Database.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class DeviceDbContext(
	DbContextOptions<DeviceDbContext> options)
	: DbContext(options)
{
    public DbSet<Device> Devices { get; set; }
    public DbSet<Component> Components { get; set; }
    public DbSet<Telemetry> Telemetries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(DatabaseConsts.Schema);

        modelBuilder.ApplyConfiguration(new DeviceConfiguration());
        modelBuilder.ApplyConfiguration(new ComponentConfiguration());
        modelBuilder.ApplyConfiguration(new TelemetryConfiguration());
    }
}
