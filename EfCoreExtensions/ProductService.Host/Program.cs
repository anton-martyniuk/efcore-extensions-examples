using Carter;
using Microsoft.EntityFrameworkCore;
using ProductService.Host.Services;
using ProductService.Infrastructure.Database;

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
	// dotnet ef migrations add Initial --startup-project ./ProductService.Host --project ./ProductService.Infrastructure -- --context ProductService.Infrastructure.Database.ApplicationDbContext

	var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
	await dbContext.Database.MigrateAsync();
	//await DatabaseSeedService.SeedAsync(dbContext);
	
	await dbContext.Products.Take(100).ToListAsync();

	await BulkOperations.InsertBulkSaveChangesAsync(dbContext);
	
	var products = await dbContext.Products.Take(100).ToListAsync();
}



await app.RunAsync();
