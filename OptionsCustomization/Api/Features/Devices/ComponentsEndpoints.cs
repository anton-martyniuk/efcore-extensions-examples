using Api.Features.Devices.Models;
using Bogus;
using Carter;
using Domain.Devices;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Devices;

public class ComponentsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/components", async (DeviceDbContext dbContext) =>
        {
            var components = await dbContext.Components.Take(10).ToListAsync();

            var response = components.Select(c => new ComponentResponse(
                c.ComponentId,
                c.DeviceId,
                c.ComponentType,
                c.Name,
                c.Capability,
                c.Unit,
                c.StateValue,
                c.State,
                c.IsActive,
                c.LastUpdatedAt
            )).ToList();

            return Results.Ok(response);
        });

	    app.MapPost("/components/efcore-insert", async (DeviceDbContext dbContext) =>
	    {
		    var deviceIds = await dbContext.Devices.Select(d => d.DeviceId).Take(10).ToListAsync();
		    var components = GenerateComponents(deviceIds, 10_000);
		    dbContext.Components.AddRange(components);

		    await dbContext.SaveChangesAsync();

		    return Results.Ok();
	    });

	    app.MapPost("/components/efcore-bulk-insert", async (DeviceDbContext dbContext) =>
	    {
		    var deviceIds = await dbContext.Devices.Select(d => d.DeviceId).Take(10).ToListAsync();
		    var components = GenerateComponents(deviceIds, 10_000);

		    await dbContext.BulkInsertOptimizedAsync(components);

		    return Results.Ok();
	    });

	    app.MapPost("/components/bulk-update", async (DeviceDbContext dbContext) =>
	    {
		    var existingComponents = await dbContext.Components.Take(100).ToListAsync();

		    // Update properties with Bogus
		    var faker = new Faker();
		    foreach (var component in existingComponents)
		    {
			    component.Name = faker.Commerce.ProductName();
			    component.State = faker.PickRandom<ComponentState>();
			    component.StateValue = faker.Random.Double(0, 100).ToString("F2");
			    component.LastUpdatedAt = faker.Date.Recent();
		    }

		    await dbContext.BulkUpdateAsync(existingComponents);

		    return Results.Ok();
	    });

	    app.MapPost("/components/bulk-delete", async (DeviceDbContext dbContext) =>
	    {
		    var existingComponents = await dbContext.Components.Take(1000).ToListAsync();

		    await dbContext.BulkDeleteAsync(existingComponents);

		    return Results.Ok();
	    });

	    app.MapPost("/components/bulk-merge", async (DeviceDbContext dbContext) =>
	    {
		    // If entity is found in the database - it is updated
		    // If entity is not found in the database - it is inserted


		    // Get some existing components for updating
		    var existingIds = await dbContext.Components.Take(50).Select(c => c.ComponentId).ToListAsync();
		    var deviceIds = await dbContext.Devices.Select(d => d.DeviceId).Take(10).ToListAsync();

		    // Generate components with mix of existing and new IDs
		    var componentsToMerge = new List<Component>();

			// Add components with existing IDs (will update)
		    foreach (var id in existingIds)
		    {
			    var faker = new Faker();
			    var deviceId = faker.PickRandom(deviceIds);
			    componentsToMerge.Add(new Component
			    {
				    ComponentId = id,
				    DeviceId = deviceId,
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

		    // Add new components (will insert)
		    var newComponents = GenerateComponents(deviceIds, 50);

		    componentsToMerge.AddRange(newComponents);

		    await dbContext.BulkMergeAsync(componentsToMerge);

		    return Results.Ok();
	    });

	    app.MapPost("/components/bulk-synchronize", async (DeviceDbContext dbContext) =>
	    {
		    // If entity is found in the database - it is updated
		    // If entity is not found in the database - it is inserted
		    // If entity is found in the database but not in the source - it is deleted

		    // Get current state of components
		    var currentIds = await dbContext.Components
			    .Select(c => c.ComponentId)
			    .Take(100)
			    .ToListAsync();

		    var deviceIds = await dbContext.Devices.Select(d => d.DeviceId).Take(10).ToListAsync();

		    // Create a desired state with some existing and some new components
		    var desiredState = new List<Component>();

		    // Keep 70% of existing components
		    var idsToKeep = currentIds.Take((int)(currentIds.Count * 0.7)).ToList();

		    foreach (var id in idsToKeep)
		    {
			    var faker = new Faker();
			    var deviceId = faker.PickRandom(deviceIds);
			    desiredState.Add(new Component
			    {
				    ComponentId = id,
				    DeviceId = deviceId,
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

		    // Add some new components
		    var newComponents = GenerateComponents(deviceIds, 30);

		    desiredState.AddRange(newComponents);

		    await dbContext.BulkSynchronizeAsync(desiredState);

		    return Results.Ok();
	    });
    }

    private static List<Component> GenerateComponents(List<long> deviceIds, int count)
    {
	    var faker = new Faker();
	    var components = new List<Component>();

	    for (var i = 0; i < count; i++)
	    {
		    var deviceId = faker.PickRandom(deviceIds);
		    components.Add(new Component
		    {
			    ComponentId = Guid.NewGuid(),
			    DeviceId = deviceId,
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
}
