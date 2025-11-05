using BenchmarkDotNet.Attributes;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductService.Domain.Products;
using ProductService.Infrastructure.Database;

namespace Benchmarks;

[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 1)]
[BenchmarkCategory("BulkInsertBenchmark")]
public class DatabaseBenchmarks
{
	private ProductDbContext _dbContext = null!;

    [GlobalSetup]
    public void Setup()
    {
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

        _dbContext = new ProductDbContext(options);
    }

    [Benchmark]
    public async Task SaveChangesAsync()
    {
        var products = GenerateProducts(10_000);
        _dbContext.Products.AddRange(products);

        await _dbContext.SaveChangesAsync();
    }

    

    private static List<Product> GenerateProducts(int count)
    {
        return new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
            .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
            .Generate(count);
    }
}
