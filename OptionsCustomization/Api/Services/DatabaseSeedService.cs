using Bogus;
using Domain.Devices;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public static class DatabaseSeedService
{
    public static async Task SeedAsync(DeviceDbContext dbContext)
    {
	    if (await dbContext.Devices.AnyAsync())
	    {
		    return;
	    }

        var devices = GenerateDevices(50);
        
        await dbContext.Devices.AddRangeAsync(devices);

        await dbContext.SaveChangesAsync();
        
        var components = GenerateComponents(devices, 150);
        var telemetries = GenerateTelemetries(devices, components, 500);
        
        await dbContext.Components.AddRangeAsync(components);
        await dbContext.Telemetries.AddRangeAsync(telemetries);

        await dbContext.SaveChangesAsync();
    }

    private static List<Device> GenerateDevices(int count)
    {
        return new Faker<Device>()
            .RuleFor(d => d.Name, f => f.Commerce.ProductName())
            .RuleFor(d => d.DeviceType, f => f.Commerce.Categories(1).First())
            .RuleFor(d => d.Manufacturer, f => f.Company.CompanyName())
            .RuleFor(d => d.SerialNumber, f => f.Random.AlphaNumeric(10))
            .RuleFor(d => d.FirmwareVersion, f => f.System.Version().ToString())
            .RuleFor(d => d.HardwareVersion, f => f.System.Version().ToString())
            .RuleFor(d => d.Status, f => f.PickRandom<DeviceStatus>())
            .RuleFor(d => d.LastSeenAt, f => f.Date.Recent())
            .RuleFor(d => d.RegisteredAt, f => f.Date.Past())
            .RuleFor(d => d.Configuration, f => f.Lorem.Sentence())
            .Generate(count);
    }

    private static List<Component> GenerateComponents(List<Device> devices, int count)
    {
        var faker = new Faker();
        var components = new List<Component>();

        for (var i = 0; i < count; i++)
        {
            var device = faker.PickRandom(devices);
            components.Add(new Component
            {
                ComponentId = Guid.NewGuid(),
                DeviceId = device.DeviceId,
                ComponentType = faker.PickRandom<ComponentType>(),
                Name = faker.Commerce.ProductName(),
                Capability = faker.Lorem.Word(),
                Unit = faker.PickRandom("°C", "°F", "kWh", "V", "A", "Pa", "%"),
                StateValue = faker.Random.Double(0, 100).ToString("F2"),
                State = faker.PickRandom<ComponentState>(),
                IsActive = faker.Random.Bool(),
                LastUpdatedAt = faker.Date.Recent()
            });
        }

        return components;
    }

    private static List<Telemetry> GenerateTelemetries(List<Device> devices, List<Component> components, int count)
    {
        var faker = new Faker();
        var telemetries = new List<Telemetry>();

        for (var i = 0; i < count; i++)
        {
            var component = faker.PickRandom(components);
            var device = devices.First(d => d.DeviceId == component.DeviceId);

            telemetries.Add(new Telemetry
            {
                TelemetryId = Guid.NewGuid(),
                DeviceId = device.DeviceId,
                ComponentId = component.ComponentId,
                Value = faker.Random.Double(0, 100),
                Quality = faker.PickRandom<TelemetryQuality>(),
                CollectedAt = faker.Date.Recent(),
                ReceivedAt = faker.Date.Recent()
            });
        }

        return telemetries;
    }
}
