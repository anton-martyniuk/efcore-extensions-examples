using System.Data.Common;
using Bogus;
using Dapper;
using ProductService.Domain.Products;
using ProductService.Domain.Users;
using ProductService.Infrastructure.Database;

namespace ProductService.Host.Services;

public static class DatabaseSeedService
{
    public static async Task SeedAsync(IConnectionFactory connectionFactory)
    {
        await using var connection = (DbConnection)connectionFactory.CreateConnection();

        const string countSql = "SELECT COUNT(1) FROM products_identity.products";
        var existing = await connection.ExecuteScalarAsync<int>(countSql);
        if (existing > 0)
        {
            return;
        }

        var users = GenerateUsers(5);
        var products = GenerateProducts(50);

        const string insertUserSql = @"
            INSERT INTO products_identity.users (username, email)
            VALUES (@Username, @Email)";

        const string insertProductSql = @"
            INSERT INTO products_identity.products
                (name, price, description, sku, barcode, category, brand, manufacturer,
                 stock_quantity, weight, is_active, created_at, updated_at)
            VALUES
                (@Name, @Price, @Description, @Sku, @Barcode, @Category, @Brand, @Manufacturer,
                 @StockQuantity, @Weight, @IsActive, @CreatedAt, @UpdatedAt)";

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            await connection.ExecuteAsync(insertUserSql, users, transaction);
            await connection.ExecuteAsync(insertProductSql, products, transaction);

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
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

    private static List<User> GenerateUsers(int count)
    {
        return new Faker<User>()
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Username, f => f.Name.FullName())
            .Generate(count);
    }
}
