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
