using Api.Services;
using Carter;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
	options.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddWebHostInfrastructure(builder.Configuration);
builder.Services.AddCarter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();

// Create and seed database
using (var scope = app.Services.CreateScope())
{
	// To create migrations run the command:
	// dotnet ef migrations add Initial --startup-project ./ProductService.Host --project ./ProductService.Infrastructure -- --context ProductService.Infrastructure.Database.IotDbContext

	var dbContext = scope.ServiceProvider.GetRequiredService<DeviceDbContext>();
	await dbContext.Database.MigrateAsync();
	//await DatabaseSeedService.SeedAsync(dbContext);
	
	await OptionsService.TestAsync(dbContext);
}



await app.RunAsync();
