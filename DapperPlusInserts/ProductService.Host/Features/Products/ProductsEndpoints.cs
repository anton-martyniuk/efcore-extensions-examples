using System.Data.Common;
using Bogus;
using Carter;
using Dapper;
using ProductService.Domain.Products;
using ProductService.Infrastructure.Database;

namespace ProductService.Host.Features.Products;

public class ProductsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products", async (IConnectionFactory connectionFactory) =>
        {
            const string sql = @"
SELECT TOP 10 id, name, price, description, sku, barcode, category, brand, manufacturer,
       stock_quantity, weight, is_active, created_at, updated_at
FROM products_identity.products";

            using var connection = connectionFactory.CreateConnection();
            var products = (await connection.QueryAsync<Product>(sql)).ToList();
            return Results.Ok(products);
        });
        
        app.MapGet("/products/{id:int}", async (int id, IProductRepository productRepository) =>
        {
            var product = await productRepository.GetByIdAsync(id);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });
        
        app.MapPost("/products/dapper-insert", async (IProductRepository productRepository) =>
        {
            var product = GenerateProducts(1).First();
            
            await productRepository.AddAsync(product);

            return Results.Ok();
        });

        app.MapPost("/products/dapper-insert-many", async (IConnectionFactory connectionFactory) =>
        {
            var products = GenerateProducts(10_000);

            const string sql = @"
INSERT INTO products_identity.products
    (name, price, description, sku, barcode, category, brand, manufacturer,
     stock_quantity, weight, is_active, created_at, updated_at)
VALUES
    (@Name, @Price, @Description, @Sku, @Barcode, @Category, @Brand, @Manufacturer,
     @StockQuantity, @Weight, @IsActive, @CreatedAt, @UpdatedAt)";

            await using var connection = (DbConnection)connectionFactory.CreateConnection();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                await connection.ExecuteAsync(sql, products, transaction);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return Results.Ok();
        });
    }

    private static List<Product> GenerateProducts(int count)
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
