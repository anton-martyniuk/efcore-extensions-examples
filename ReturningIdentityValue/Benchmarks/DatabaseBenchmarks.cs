using BenchmarkDotNet.Attributes;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductService.Domain.Products;
using ProductService.Domain.Users;
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
    
    [Benchmark]
    public async Task BulkInsertAsync()
    {
        var products = GenerateProducts(10_000);

        await _dbContext.BulkInsertAsync(products);
    }

    
    [Benchmark]
    public async Task BulkInsertOptimizedAsync()
    {
        var products = GenerateProducts(10_000);

        await _dbContext.BulkInsertOptimizedAsync(products);
    }
    
    [Benchmark]
    public async Task InsertGraph_SaveChangesAsync()
    {
        var productCarts = GenerateProductCarts(10_000);
        _dbContext.ProductCarts.AddRange(productCarts);

        await _dbContext.SaveChangesAsync();
    }

    [Benchmark]
    public async Task InsertGraph_BulkInsertAsync()
    {
        var productCarts = GenerateProductCarts(10_000);
        await _dbContext.BulkInsertAsync(productCarts, options => options.IncludeGraph = true);
    }
    
    [Benchmark]
    public async Task InsertGraph_BulkInsertOptimizedAsync()
    {
        var productCarts = GenerateProductCarts(10_000);
        await _dbContext.BulkInsertOptimizedAsync(productCarts, options => options.IncludeGraph = true);
    }
    

    public static List<Product> GenerateProducts(int count)
    {
        return new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
            .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
            .Generate(count);
    }
    
    private static List<ProductCart> GenerateProductCarts(int count)
    {
        var users = new Faker<User>()
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .Generate(100);

        var products = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
            .Generate(200);

        return new Faker<ProductCart>()
            .RuleFor(pc => pc.Quantity, f => f.Random.Int(1, 10))
            .RuleFor(pc => pc.User, f =>  f.PickRandom(users))
            .RuleFor(pc => pc.CreatedOn, f => f.Date.Recent(30))
            .RuleFor(pc => pc.CartItems, (f, pc) =>
            {
                var cartItems = new List<ProductCartItem>();
                var itemCount = f.Random.Int(1, 5);

                for (var i = 0; i < itemCount; i++)
                {
                    var product = f.PickRandom(products);
                    cartItems.Add(new ProductCartItem
                    {
                        ProductCartId = pc.Id,
                        ProductId = product.Id,
                        Product = product,
                        Quantity = f.Random.Int(1, 5)
                    });
                }

                return cartItems;
            })
            .Generate(count);
    }
}
