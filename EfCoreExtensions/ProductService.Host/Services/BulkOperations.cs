using System.Diagnostics;
using Bogus;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Products;
using ProductService.Domain.Users;
using ProductService.Infrastructure.Database;

namespace ProductService.Host.Services;

public static class BulkOperations
{
    public static async Task InsertSaveChangesAsync(ProductDbContext dbContext)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var products = GenerateProducts(10_000);
        dbContext.Products.AddRange(products);

        await dbContext.SaveChangesAsync(); // 4.5 seconds
        
        stopwatch.Stop();
        var elapsed = stopwatch.ElapsedMilliseconds;
    }
    
    public static async Task InsertBulkSaveChangesAsync(ProductDbContext dbContext)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var products = GenerateProducts(10_000);
        dbContext.Products.AddRange(products);

        await dbContext.BulkSaveChangesAsync();
        
        stopwatch.Stop();
        var elapsed = stopwatch.ElapsedMilliseconds; // 0.9 seconds
    }
    
    public static async Task InsertBulkAsync(ProductDbContext dbContext)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var products = GenerateProducts(10);
        await dbContext.BulkInsertAsync(products);
        
        stopwatch.Stop();
        var elapsed = stopwatch.ElapsedMilliseconds; // 0,6 seconds
    }
    
    public static async Task InsertBulkOptimizedAsync(ProductDbContext dbContext)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var products = GenerateProducts(10_000);
        var response = await dbContext.BulkInsertOptimizedAsync(products);
        
        stopwatch.Stop();
        var elapsed = stopwatch.ElapsedMilliseconds; // 0,467 seconds
    }
    
    public static async Task InsertBulkOptimizeGraphAsync(ProductDbContext dbContext)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var products = GenerateProductCarts(10_000);
        var response = await dbContext.BulkInsertOptimizedAsync(products, options =>
        {
            options.IncludeGraph = true;
        });
        
        stopwatch.Stop();
        var elapsed = stopwatch.ElapsedMilliseconds;
    }
    
    public static async Task BulkUpdateAsync(ProductDbContext dbContext)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var existingProducts = dbContext.Products.Take(100).ToList();
        
        var faker = new Faker();
        foreach (var product in existingProducts)
        {
            product.Name = faker.Commerce.ProductName();
            product.Description = faker.Commerce.ProductDescription();
            product.Price = decimal.Parse(faker.Commerce.Price());
        }
        
        await dbContext.BulkUpdateAsync(existingProducts, options =>
        {
            options.IgnoreOnUpdateExpression = p => p.Id;
            options.ColumnPrimaryKeyExpression = p => new { p.Name, p.Id };
            options.IncludeGraph = true;
        });
        
        stopwatch.Stop();
        var elapsed = stopwatch.ElapsedMilliseconds; // 0,467 seconds
    }
    
    public static async Task BulkDeleteAsync(ProductDbContext dbContext)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var existingProducts = dbContext.Products.Take(1000).ToList();
        
        await dbContext.BulkDeleteAsync(existingProducts);
        
        stopwatch.Stop();
        var elapsed = stopwatch.ElapsedMilliseconds; // 0,467 seconds
    }
    
    public static async Task BulkMergeAsync(ProductDbContext dbContext)
    {
        var products = await dbContext.Products.AsNoTracking().ToListAsync();

        var existingIds = await dbContext.Products.Take(5).Select(p => p.Id).ToListAsync();
		    
        // Generate products with mix of existing and new IDs
        var productsToMerge = new List<Product>();

        // Add products with existing IDs (will update)
        foreach (var id in existingIds)
        {
            productsToMerge.Add(new Product
            {
                Id = id,
                Name = new Faker().Commerce.ProductName(),
                Description = new Faker().Lorem.Paragraph(),
                Price = decimal.Parse(new Faker().Commerce.Price())
            });
        }
		    
        // Add new products (will insert)
        var newProducts = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
            .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
            .Generate(5);

        productsToMerge.AddRange(newProducts);

        await dbContext.BulkMergeAsync(productsToMerge);
        
        products = await dbContext.Products.AsNoTracking().ToListAsync();
    }

    public static async Task BulkSyncAsync(ProductDbContext dbContext)
    {
        // 15 products
        var products = await dbContext.Products.AsNoTracking().ToListAsync();

        // Read the first 5 products
        var existingIds = await dbContext.Products.Take(5).Select(p => p.Id).ToListAsync();
		    
        // Generate products with mix of existing and new IDs
        var productsToSync = new List<Product>();

        // Update the first 5 products
        foreach (var id in existingIds)
        {
            productsToSync.Add(new Product
            {
                Id = id,
                Name = new Faker().Commerce.ProductName(),
                Description = new Faker().Lorem.Paragraph(),
                Price = decimal.Parse(new Faker().Commerce.Price())
            });
        }
		    
        // Add new 5 products
        var newProducts = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
            .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
            .Generate(5);

        productsToSync.AddRange(newProducts);

        // Insert, Update, Delete
        await dbContext.BulkSynchronizeAsync(productsToSync);
        
        // Insert 5 new products
        // Update 5 existing products
        // Delete 5 existing products
        
        products = await dbContext.Products.AsNoTracking().ToListAsync();
    }
    
    private static List<Product> GenerateProducts(int count)
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
            .RuleFor(pc => pc.Id, _ => Guid.NewGuid())
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