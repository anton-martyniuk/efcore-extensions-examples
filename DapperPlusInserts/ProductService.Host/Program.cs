using Carter;
using ProductService.Domain.Products;
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

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

using (var scope = app.Services.CreateScope())
{
    var connectionFactory = scope.ServiceProvider.GetRequiredService<IConnectionFactory>();
    await DatabaseInitializer.InitializeAsync(connectionFactory);
    DapperPlusMapping.Map();
    
    var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
    var product = DatabaseSeedService.GenerateProducts(1).First();

    await repository.AddWithDapperPlusAsync(product);
}

await app.RunAsync();
