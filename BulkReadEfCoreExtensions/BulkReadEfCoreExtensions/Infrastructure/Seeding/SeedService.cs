using Bogus;
using BulkReadEfCoreExtensions.Domain.Entities;
using BulkReadEfCoreExtensions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BulkReadEfCoreExtensions.Infrastructure.Seeding;

public class SeedService(ShippingDbContext context)
{
    public async Task SeedDataAsync()
    {
        if (await context.Products.AnyAsync())
        {
            return;
        }

        var fakeCategories = new Faker<Category>()
            .CustomInstantiator(f => new Category
            {
                Name = f.Commerce.Categories(1)[0]
            });

        var categories = fakeCategories.Generate(10);
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        var fakeProducts = new Faker<Product>()
            .CustomInstantiator(f => new Product
            {
                ProductCode = f.Commerce.Ean8(),
                SupplierCode = f.Commerce.Ean13(),
                Name = f.Commerce.ProductName(),
                Price = decimal.Parse(f.Commerce.Price()),
                Stock = f.Random.Int(0, 1000),
                IsActive = f.Random.Bool(0.9f),
                Category = categories[f.Random.Int(0, categories.Count - 1)],
                Items = Enumerable.Range(1, f.Random.Int(1, 5))
                    .Select(_ => new ProductItem
                    {
                        Sku = f.Commerce.Ean13(),
                        Quantity = f.Random.Int(1, 5)
                    })
                    .ToList()
            });

        var products = fakeProducts.Generate(10_000);
        context.Products.AddRange(products);
        await context.SaveChangesAsync();
    }
}
