using System.Diagnostics;
using System.Text;
using Bogus;
using Domain.Devices;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;
using Z.EntityFramework.Extensions;

namespace Api.Services;

public static class OptionsService
{
    public static async Task TestAsync(DeviceDbContext dbContext)
    {
        Randomizer.Seed = new Random(1283);
        
        // var devices = GenerateDevices(10);
        // await dbContext.BulkInsertAsync(devices);
        //
        // // Insert same 10 devices again with InsertIfNotExists = true option
        // await dbContext.BulkInsertAsync(devices, options =>
        // {
        //     options.InsertIfNotExists = true;
        // });
        
        // var devices = GenerateDevices(2);
        //
        // devices[0].DeviceId = 1000;
        // devices[1].DeviceId = 1001;
        //
        // await dbContext.BulkInsertAsync(devices, options =>
        // {
        //     options.InsertKeepIdentity = true;
        // });

        //await MeasureBulkInsertPerformance(dbContext);

        // EntityFrameworkManager.PreBulkMerge = (ctx, obj) =>
        // {
        //     if (obj is IEnumerable<Device> list)
        //     {
        //         foreach (var customer in list)
        //         {
        //             if (customer.DeviceType == "Electronics")
        //             {
        //                 customer.DeviceType = "Electronics 2.0";
        //             }
        //         }
        //     }
        // };

        // await InsertDevicesWithComponents(dbContext);
        // await MeasureBulkInsertPerformance(dbContext);
        //await InsertDevicesWithTruncatedHardwareVersion(dbContext);
        // await DefineInputOutputExpressions(dbContext, devices);
        await UpdateDevicePropertiesWithCoalesce(dbContext);
        // await UpdateDevicePropertiesMatched(dbContext);
        // await MergeDevicesAndComponents(dbContext);
        //await Auditing(dbContext);
    }

    private static async Task Auditing(DeviceDbContext dbContext)
    {
        var existingDevices = await dbContext.Devices
            .Include(x => x.Components)
            .Take(10).ToListAsync();
        
        var faker = new Faker();
        foreach (var device in existingDevices)
        {
            device.Name = faker.Commerce.ProductName();
            device.Status = faker.PickRandom<DeviceStatus>();
            device.LastSeenAt = faker.Date.Recent();
            device.RegisteredAt = faker.Date.Recent();
        }
        
        existingDevices.AddRange(GenerateDevices(5));
        
        var auditEntries = new List<AuditEntry>();

        var builder = new StringBuilder();
        
        var resultInfo = new Z.BulkOperations.ResultInfo();
        
        await dbContext.BulkMergeAsync(existingDevices, options =>
        {
            options.IncludeGraph = true;
            options.UseAudit = true;
            options.AuditEntries = auditEntries;
            
            options.Log += message => builder.AppendLine(message);

            options.ResultInfo = resultInfo;
        });
        
        Console.WriteLine(builder.ToString());
    }

    private static async Task MergeDevicesAndComponents(DeviceDbContext dbContext)
    {
        var existingDevices = await dbContext.Devices
            .Include(x => x.Components)
            .Take(10).ToListAsync();
        
        var faker = new Faker();
        foreach (var device in existingDevices)
        {
            device.Name = faker.Commerce.ProductName();
            device.Status = faker.PickRandom<DeviceStatus>();
            device.LastSeenAt = faker.Date.Recent();
            device.RegisteredAt = faker.Date.Recent();
        }
        
        existingDevices.AddRange(GenerateDevices(5));
        
        await dbContext.BulkMergeAsync(existingDevices, options =>
        {
            options.IncludeGraph = true;
            options.IncludeGraphOperationBuilder = operation =>
            {
                if (operation is BulkOperation<Device> bulkOperation)
                {
                    bulkOperation.ColumnPrimaryKeyExpression = x => x.SerialNumber;
                }
                else if (operation is BulkOperation<Component> bulk)
                {
                    bulk.ColumnPrimaryKeyExpression = x => x.Name;
                }
            };
        });
    }

    private static async Task UpdateDevicePropertiesMatched(DeviceDbContext dbContext)
    {
        var existingDevices = await dbContext.Devices.Take(10).ToListAsync();

        var faker = new Faker();
        foreach (var device in existingDevices)
        {
            device.Name = faker.Commerce.ProductName();
            device.Status = faker.PickRandom<DeviceStatus>();
            device.LastSeenAt = faker.Date.Recent();
            device.RegisteredAt = faker.Date.Recent();
        }
        
        existingDevices[0].DeviceType = "new device type";
        existingDevices[1].DeviceType = "new device type";

        await dbContext.BulkUpdateAsync(existingDevices, options =>
        {
            // Status, Date, Row version, IsLocked and IsDeleted
            options.UpdateMatchedAndConditionExpression = d => new { d.DeviceType };
            
            // Updating only important fields. Audit (ModifiedDate, ModifiedBy)
            options.UpdateMatchedAndOneNotConditionExpression = d => new { d.DeviceType, d.SerialNumber };
        });
    }

    private static async Task UpdateDevicePropertiesWithCoalesce(DeviceDbContext dbContext)
    {
        var existingDevices = await dbContext.Devices.Take(10).ToListAsync();

        var faker = new Faker();
        foreach (var device in existingDevices)
        {
            device.Name = faker.Commerce.ProductName();
            device.Status = faker.PickRandom<DeviceStatus>();
            device.LastSeenAt = faker.Date.Recent();
            device.RegisteredAt = faker.Date.Recent();
            device.SerialNumber = null;
            device.Configuration = "configuration";
        }

        await dbContext.BulkUpdateAsync(existingDevices, options =>
        {
            options.IgnoreOnUpdateExpression = d => d.RegisteredAt;
            options.CoalesceOnUpdateExpression = d => d.SerialNumber;
            options.CoalesceDestinationOnUpdateExpression = d => d.Configuration;
        });
    }

    private static async Task DefineInputOutputExpressions(DeviceDbContext dbContext, List<Device> devices)
    {
        await dbContext.BulkInsertAsync(devices, options =>
        {
            // Input, Output, Ignore
            
            options.ColumnInputExpression = d => new { d.Name, d.DeviceType, d.Manufacturer, d.SerialNumber };
            options.ColumnOutputExpression = d => new { d.DeviceId };
            
            options.IgnoreOnInsertExpression = d => d.LastSeenAt;
        });
    }

    private static async Task InsertDevicesWithTruncatedHardwareVersion(DeviceDbContext dbContext)
    {
        var devices = GenerateDevices(10);
        
        foreach (var device in devices)
        {
            device.HardwareVersion += "some long text that needs to be truncated by EF Core Extensions library";
        }
        
        await dbContext.BulkInsertAsync(devices, options =>
        {
            options.AutoTruncate = true;
        });
    }

    private static async Task InsertDevicesWithComponents(DeviceDbContext dbContext)
    {
        var devices = GenerateDevices(10);
        
        foreach (var device in devices)
        {
            device.Components = GenerateComponents(3);
        }
        
        await dbContext.BulkInsertAsync(devices, options => options.IncludeGraph = true);
    }

    private static async Task MeasureBulkInsertPerformance(DeviceDbContext dbContext)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        
        var devices = GenerateDevices(10_000);
        
        await dbContext.BulkInsertAsync(devices, options =>
        {
            options.AutoMapOutputDirection = true;
        });
        
        stopWatch.Stop();
        var elapsed = stopWatch.ElapsedMilliseconds;
        
        stopWatch.Start();
        
        devices = GenerateDevices(10_000);
        
        await dbContext.BulkInsertOptimizedAsync(devices);
        stopWatch.Stop();
        
        var elapsed2 = stopWatch.ElapsedMilliseconds;
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
    
    private static List<Component> GenerateComponents(int count)
    {
        var faker = new Faker();
        var components = new List<Component>();

        for (var i = 0; i < count; i++)
        {
            components.Add(new Component
            {
                ComponentId = Guid.NewGuid(),
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