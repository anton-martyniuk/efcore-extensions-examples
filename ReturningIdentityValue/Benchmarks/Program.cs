using BenchmarkDotNet.Running;
using Benchmarks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductService.Infrastructure.Database;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .Build();

var connectionString = configuration.GetConnectionString("SqlServer");
        
var options = new DbContextOptionsBuilder<ProductDbContext>()
    .UseSqlServer(connectionString)
    .UseSnakeCaseNamingConvention()
    .Options;

await using (var dbContext = new ProductDbContext(options))
{
    var products = DatabaseBenchmarks.GenerateProducts(10_000);
    //dbContext.Products.AddRange(products);

    await dbContext.BulkInsertOptimizedAsync(products);
}

BenchmarkRunner.Run<DatabaseBenchmarks>();