using BulkReadEfCoreExtensions.Infrastructure;
using BulkReadEfCoreExtensions.Infrastructure.Persistence;
using BulkReadEfCoreExtensions.Infrastructure.Seeding;
using BulkReadEfCoreExtensions.WebApi.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ShippingDbContext>();
    await dbContext.Database.MigrateAsync();

    var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
    await seedService.SeedDataAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapFeatures();

app.Run();
