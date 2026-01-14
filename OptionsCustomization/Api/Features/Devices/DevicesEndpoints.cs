using System.Diagnostics;
using Api.Features.Devices.Models;
using Bogus;
using Carter;
using Domain.Devices;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Devices;

public class DevicesEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/devices", async (DeviceDbContext dbContext) =>
        {
            var devices = await dbContext.Devices.Take(10).ToListAsync();

            var response = devices.Select(d => new DeviceResponse(
                d.DeviceId,
                d.Name,
                d.DeviceType,
                d.Manufacturer,
                d.SerialNumber,
                d.FirmwareVersion,
                d.HardwareVersion,
                d.Status,
                d.LastSeenAt,
                d.RegisteredAt,
                d.Configuration
            )).ToList();

            return Results.Ok(response);
        });

	    app.MapPost("/devices/efcore-insert", async (DeviceDbContext dbContext) =>
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();

		    var devices = GenerateDevices(10_000);
		    dbContext.Devices.AddRange(devices);

		    await dbContext.SaveChangesAsync();

		    stopwatch.Stop();
		    var totalMilliseconds = stopwatch.ElapsedMilliseconds;

		    return Results.Ok();
	    });

	    app.MapPost("/devices/efcore-bulk-insert", async (DeviceDbContext dbContext) =>
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();

		    var devices = GenerateDevices(10_000);

		    await dbContext.BulkInsertOptimizedAsync(devices);

		    stopwatch.Stop();
		    var totalMilliseconds = stopwatch.ElapsedMilliseconds;

		    return Results.Ok();
	    });

	    app.MapPost("/devices/bulk-update", async (DeviceDbContext dbContext) =>
	    {
		    var existingDevices = await dbContext.Devices.Take(100).ToListAsync();

		    var faker = new Faker();
		    foreach (var device in existingDevices)
		    {
			    device.Name = faker.Commerce.ProductName();
			    device.Status = faker.PickRandom<DeviceStatus>();
			    device.LastSeenAt = faker.Date.Recent();
		    }

		    await dbContext.BulkUpdateAsync(existingDevices);

		    return Results.Ok();
	    });

	    app.MapPost("/devices/bulk-delete", async (DeviceDbContext dbContext) =>
	    {
		    var existingDevices = await dbContext.Devices.Take(1000).ToListAsync();

		    await dbContext.BulkDeleteAsync(existingDevices);

		    return Results.Ok();
	    });

	    app.MapPost("/devices/bulk-merge", async (DeviceDbContext dbContext) =>
	    {
		    // If entity is found in the database - it is updated
		    // If entity is not found in the database - it is inserted


		    // Get some existing devices for updating
		    var existingIds = await dbContext.Devices.Take(50).Select(d => d.DeviceId).ToListAsync();

		    // Generate devices with mix of existing and new IDs
		    var devicesToMerge = new List<Device>();

			// Add devices with existing IDs (will update)
		    foreach (var id in existingIds)
		    {
			    var faker = new Faker();
			    devicesToMerge.Add(new Device
			    {
				    DeviceId = id,
				    Name = faker.Commerce.ProductName(),
				    DeviceType = faker.Commerce.Categories(1).First(),
				    Manufacturer = faker.Company.CompanyName(),
				    SerialNumber = faker.Random.AlphaNumeric(10),
				    FirmwareVersion = faker.System.Version().ToString(),
				    HardwareVersion = faker.System.Version().ToString(),
				    Status = faker.PickRandom<DeviceStatus>(),
				    LastSeenAt = faker.Date.Recent(),
				    RegisteredAt = faker.Date.Past(),
				    Configuration = faker.Lorem.Sentence()
			    });
		    }

		    // Add new devices (will insert)
		    var newDevices = GenerateDevices(50);

		    devicesToMerge.AddRange(newDevices);

		    await dbContext.BulkMergeAsync(devicesToMerge);

		    return Results.Ok();
	    });

	    app.MapPost("/devices/bulk-synchronize", async (DeviceDbContext dbContext) =>
	    {
		    // If entity is found in the database - it is updated
		    // If entity is not found in the database - it is inserted
		    // If entity is found in the database but not in the source - it is deleted

		    // Get current state of devices
		    var currentIds = await dbContext.Devices
			    .Select(d => d.DeviceId)
			    .Take(100)
			    .ToListAsync();

		    // Create a desired state with some existing and some new devices
		    var desiredState = new List<Device>();

		    // Keep 70% of existing devices
		    var idsToKeep = currentIds.Take((int)(currentIds.Count * 0.7)).ToList();

		    foreach (var id in idsToKeep)
		    {
			    var faker = new Faker();
			    desiredState.Add(new Device
			    {
				    DeviceId = id,
				    Name = faker.Commerce.ProductName(),
				    DeviceType = faker.Commerce.Categories(1).First(),
				    Manufacturer = faker.Company.CompanyName(),
				    SerialNumber = faker.Random.AlphaNumeric(10),
				    FirmwareVersion = faker.System.Version().ToString(),
				    HardwareVersion = faker.System.Version().ToString(),
				    Status = faker.PickRandom<DeviceStatus>(),
				    LastSeenAt = faker.Date.Recent(),
				    RegisteredAt = faker.Date.Past(),
				    Configuration = faker.Lorem.Sentence()
			    });
		    }

		    // Add some new devices
		    var newDevices = GenerateDevices(30);

		    desiredState.AddRange(newDevices);

		    await dbContext.BulkSynchronizeAsync(desiredState);

		    return Results.Ok();
	    });
    }

    private static List<Device> GenerateDevices(int count)
    {
	    return new Faker<Device>()
		    .RuleFor(d => d.DeviceId, f => f.Random.Long(1, long.MaxValue))
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
}
