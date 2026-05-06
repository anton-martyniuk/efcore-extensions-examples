using System.Data.Common;
using BenchmarkDotNet.Attributes;
using Bogus;
using Dapper;
using Microsoft.Extensions.Configuration;
using ProductService.Domain.Products;
using ProductService.Infrastructure.Database;
using Z.Dapper.Plus;

namespace Benchmarks;

[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 1)]
[BenchmarkCategory("BulkInsertBenchmark")]
public class DatabaseBenchmarks
{
    private IConnectionFactory _connectionFactory = null!;

    [GlobalSetup]
    public void Setup()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Missing connection string 'SqlServer'.");

        _connectionFactory = new SqlConnectionFactory(connectionString);

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        DapperPlusManager.Entity<Product>()
            .Table("products_identity.products")
            .Identity(x => x.Id)
            .Map(x => x.Id, "id")
            .Map(x => x.Name, "name")
            .Map(x => x.Price, "price")
            .Map(x => x.Description, "description")
            .Map(x => x.Sku, "sku")
            .Map(x => x.Barcode, "barcode")
            .Map(x => x.Category, "category")
            .Map(x => x.Brand, "brand")
            .Map(x => x.Manufacturer, "manufacturer")
            .Map(x => x.StockQuantity, "stock_quantity")
            .Map(x => x.Weight, "weight")
            .Map(x => x.IsActive, "is_active")
            .Map(x => x.CreatedAt, "created_at")
            .Map(x => x.UpdatedAt, "updated_at");
    }

    [Benchmark]
    public async Task InsertAsync()
    {
        var products = GenerateProducts(10_000);

        const string sql = @"
            INSERT INTO products_identity.products
                (name, price, description, sku, barcode, category, brand, manufacturer,
                 stock_quantity, weight, is_active, created_at, updated_at)
            VALUES
                (@Name, @Price, @Description, @Sku, @Barcode, @Category, @Brand, @Manufacturer,
                 @StockQuantity, @Weight, @IsActive, @CreatedAt, @UpdatedAt)";

        await using var connection = (DbConnection)_connectionFactory.CreateConnection();
        await using var transaction = await connection.BeginTransactionAsync();

        await connection.ExecuteAsync(sql, products, transaction);
        await transaction.CommitAsync();
    }

    [Benchmark]
    public async Task BulkInsertAsync()
    {
        var products = GenerateProducts(10_000);

        using var connection = _connectionFactory.CreateConnection();
        await connection.BulkInsertAsync(products);
    }

    public static List<Product> GenerateProducts(int count)
    {
        return new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
            .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
            .RuleFor(p => p.Sku, f => $"SKU-{Guid.NewGuid():N}")
            .RuleFor(p => p.Barcode, f => f.Commerce.Ean8())
            .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
            .RuleFor(p => p.Brand, f => f.Company.CompanyName())
            .RuleFor(p => p.Manufacturer, f => f.Company.CompanyName())
            .RuleFor(p => p.StockQuantity, f => f.Random.Int(0, 10_000))
            .RuleFor(p => p.Weight, f => Math.Round(f.Random.Decimal(0.01m, 50m), 3))
            .RuleFor(p => p.IsActive, f => f.Random.Bool(0.9f))
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(2).ToUniversalTime())
            .RuleFor(p => p.UpdatedAt, f => f.Date.Recent(30).OrNull(f, 0.3f)?.ToUniversalTime())
            .Generate(count);
    }
}
