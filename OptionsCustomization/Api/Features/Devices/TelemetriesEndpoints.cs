using System.Diagnostics;
using Api.Features.Devices.Models;
using Bogus;
using Carter;
using Domain.Devices;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Devices;

public class TelemetriesEndpoints : ICarterModule
{
    private record DeviceComponentPair(long DeviceId, Guid ComponentId);
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/telemetries", async (DeviceDbContext dbContext) =>
        {
            var telemetries = await dbContext.Telemetries.Take(10).ToListAsync();

            var response = telemetries.Select(t => new TelemetryResponse(
                t.TelemetryId,
                t.DeviceId,
                t.ComponentId,
                t.Value,
                t.Quality,
                t.CollectedAt,
                t.ReceivedAt
            )).ToList();

            return Results.Ok(response);
        });

	    app.MapPost("/telemetries/efcore-insert", async (DeviceDbContext dbContext) =>
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();

		    var deviceComponentPairs = await dbContext.Components
			    .Take(10)
			    .Select(c => new DeviceComponentPair(c.DeviceId, c.ComponentId))
			    .ToListAsync();

		    var telemetries = GenerateTelemetries(deviceComponentPairs, 10_000);
		    dbContext.Telemetries.AddRange(telemetries);

		    await dbContext.SaveChangesAsync();

		    stopwatch.Stop();
		    var totalMilliseconds = stopwatch.ElapsedMilliseconds;

		    return Results.Ok();
	    });

	    app.MapPost("/telemetries/efcore-bulk-insert", async (DeviceDbContext dbContext) =>
	    {
		    var stopwatch = new Stopwatch();
		    stopwatch.Start();

		    var deviceComponentPairs = await dbContext.Components
			    .Take(10)
			    .Select(c => new DeviceComponentPair(c.DeviceId, c.ComponentId))
			    .ToListAsync();

		    var telemetries = GenerateTelemetries(deviceComponentPairs, 10_000);

		    await dbContext.BulkInsertOptimizedAsync(telemetries);

		    stopwatch.Stop();
		    var totalMilliseconds = stopwatch.ElapsedMilliseconds;

		    return Results.Ok();
	    });

	    app.MapPost("/telemetries/bulk-update", async (DeviceDbContext dbContext) =>
	    {
		    var existingTelemetries = await dbContext.Telemetries.Take(100).ToListAsync();

		    // Update properties with Bogus
		    var faker = new Faker();
		    foreach (var telemetry in existingTelemetries)
		    {
			    telemetry.Value = faker.Random.Double(0, 100);
			    telemetry.Quality = faker.PickRandom<TelemetryQuality>();
			    telemetry.ReceivedAt = faker.Date.Recent();
		    }

		    await dbContext.BulkUpdateAsync(existingTelemetries);

		    return Results.Ok();
	    });

	    app.MapPost("/telemetries/bulk-delete", async (DeviceDbContext dbContext) =>
	    {
		    var existingTelemetries = await dbContext.Telemetries.Take(1000).ToListAsync();

		    await dbContext.BulkDeleteAsync(existingTelemetries);

		    return Results.Ok();
	    });

	    app.MapPost("/telemetries/bulk-merge", async (DeviceDbContext dbContext) =>
	    {
		    // If entity is found in the database - it is updated
		    // If entity is not found in the database - it is inserted


		    // Get some existing telemetries for updating
		    var existingIds = await dbContext.Telemetries.Take(50).Select(t => t.TelemetryId).ToListAsync();
		    var deviceComponentPairs = await dbContext.Components
			    .Take(10)
			    .Select(c => new DeviceComponentPair(c.DeviceId, c.ComponentId))
			    .ToListAsync();

		    // Generate telemetries with mix of existing and new IDs
		    var telemetriesToMerge = new List<Telemetry>();

			// Add telemetries with existing IDs (will update)
		    foreach (var id in existingIds)
		    {
			    var faker = new Faker();
			    var pair = faker.PickRandom(deviceComponentPairs);
			    telemetriesToMerge.Add(new Telemetry
			    {
				    TelemetryId = id,
				    DeviceId = pair.DeviceId,
				    ComponentId = pair.ComponentId,
				    Value = faker.Random.Double(0, 100),
				    Quality = faker.PickRandom<TelemetryQuality>(),
				    CollectedAt = faker.Date.Recent(),
				    ReceivedAt = faker.Date.Recent()
			    });
		    }

		    // Add new telemetries (will insert)
		    var newTelemetries = GenerateTelemetries(deviceComponentPairs, 50);

		    telemetriesToMerge.AddRange(newTelemetries);

		    await dbContext.BulkMergeAsync(telemetriesToMerge);

		    return Results.Ok();
	    });

	    app.MapPost("/telemetries/bulk-synchronize", async (DeviceDbContext dbContext) =>
	    {
		    // If entity is found in the database - it is updated
		    // If entity is not found in the database - it is inserted
		    // If entity is found in the database but not in the source - it is deleted

		    // Get current state of telemetries
		    var currentIds = await dbContext.Telemetries
			    .Select(t => t.TelemetryId)
			    .Take(100)
			    .ToListAsync();

		    var deviceComponentPairs = await dbContext.Components
			    .Take(10)
			    .Select(c => new DeviceComponentPair(c.DeviceId, c.ComponentId))
			    .ToListAsync();

		    // Create a desired state with some existing and some new telemetries
		    var desiredState = new List<Telemetry>();

		    // Keep 70% of existing telemetries
		    var idsToKeep = currentIds.Take((int)(currentIds.Count * 0.7)).ToList();

		    foreach (var id in idsToKeep)
		    {
			    var faker = new Faker();
			    var pair = faker.PickRandom(deviceComponentPairs);
			    desiredState.Add(new Telemetry
			    {
				    TelemetryId = id,
				    DeviceId = pair.DeviceId,
				    ComponentId = pair.ComponentId,
				    Value = faker.Random.Double(0, 100),
				    Quality = faker.PickRandom<TelemetryQuality>(),
				    CollectedAt = faker.Date.Recent(),
				    ReceivedAt = faker.Date.Recent()
			    });
		    }

		    // Add some new telemetries
		    var newTelemetries = GenerateTelemetries(deviceComponentPairs, 30);

		    desiredState.AddRange(newTelemetries);

		    await dbContext.BulkSynchronizeAsync(desiredState);

		    return Results.Ok();
	    });
    }

    private static List<Telemetry> GenerateTelemetries(List<DeviceComponentPair> deviceComponentPairs, int count)
    {
	    var faker = new Faker();
	    var telemetries = new List<Telemetry>();

	    for (var i = 0; i < count; i++)
	    {
		    var pair = faker.PickRandom(deviceComponentPairs);
		    telemetries.Add(new Telemetry
		    {
			    TelemetryId = Guid.NewGuid(),
			    DeviceId = pair.DeviceId,
			    ComponentId = pair.ComponentId,
			    Value = faker.Random.Double(0, 100),
			    Quality = faker.PickRandom<TelemetryQuality>(),
			    CollectedAt = faker.Date.Recent(),
			    ReceivedAt = faker.Date.Recent()
		    });
	    }

	    return telemetries;
    }
}
