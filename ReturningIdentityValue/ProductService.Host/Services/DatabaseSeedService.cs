using Bogus;
using ProductService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Products;
using ProductService.Domain.Users;

namespace ProductService.Host.Services;

public static class DatabaseSeedService
{
    public static async Task SeedAsync(ProductDbContext dbContext)
    {
	    if (await dbContext.Products.AnyAsync())
	    {
		    return;
	    }

        var users = GenerateUsers(5);
        var products = GenerateProducts(50);

        await dbContext.Users.AddRangeAsync(users);
        await dbContext.Products.AddRangeAsync(products);

        await dbContext.SaveChangesAsync();
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

    private static List<User> GenerateUsers(int count)
    {
        return new Faker<User>()
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Username, f => f.Name.FullName())
            .Generate(count);
    }
}
