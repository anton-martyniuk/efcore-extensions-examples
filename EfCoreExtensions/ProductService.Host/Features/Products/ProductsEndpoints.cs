using Bogus;
using Carter;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ProductService.Domain.Products;
using ProductService.Infrastructure.Database;

namespace ProductService.Host.Features.Products;

public class ProductsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products", async (ProductDbContext dbContext) =>
        {
            var products = await dbContext.Products.Take(10).ToListAsync();
            return Results.Ok(products);
        });
        
	    app.MapPost("/products/efcore-insert", async (ProductDbContext dbContext) =>
	    {
		    var products = GenerateProducts(10_000);
		    dbContext.Products.AddRange(products);

		    await dbContext.SaveChangesAsync();

		    return Results.Ok();
	    });
	    
	    app.MapPost("/products/efcore-bulk-insert", async (ProductDbContext dbContext) =>
	    {
		    var products = GenerateProducts(10_000);

		    await dbContext.BulkInsertOptimizedAsync(products);

		    return Results.Ok();
	    });
	    
	    app.MapPost("/products/bulk-update", async (ProductDbContext dbContext) =>
	    {
		    var existingProducts = await dbContext.Products.Take(100).ToListAsync();

		    // Update properties with Bogus
		    var faker = new Faker();
		    foreach (var product in existingProducts)
		    {
			    product.Name = faker.Commerce.ProductName();
			    product.Description = faker.Commerce.ProductDescription();
			    product.Price = decimal.Parse(faker.Commerce.Price());
		    }
		    
		    await dbContext.BulkUpdateAsync(existingProducts);

		    return Results.Ok();
	    });
	    
	    app.MapPost("/products/bulk-delete", async (ProductDbContext dbContext) =>
	    {
		    var existingProducts = await dbContext.Products.Take(1000).ToListAsync();
		    
		    await dbContext.BulkDeleteAsync(existingProducts);

		    return Results.Ok();
	    });
	    
	    app.MapPost("/products/bulk-merge", async (ProductDbContext dbContext) =>
	    {
		    // If entity is found in the database - it is updated
		    // If entity is not found in the database - it is inserted
		    
		    
		    // Get some existing products for updating
		    var existingIds = await dbContext.Products.Take(50).Select(p => p.Id).ToListAsync();
		    
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
			    .Generate(50);

		    productsToMerge.AddRange(newProducts);

		    await dbContext.BulkMergeAsync(productsToMerge);

		    return Results.Ok();
	    });
	    
	    app.MapPost("/products/bulk-synchronize", async (ProductDbContext dbContext) =>
	    {
		    // If entity is found in the database - it is updated
		    // If entity is not found in the database - it is inserted
		    // If entity is found in the database but not in the source - it is deleted
		    
		    // Get current state of products
		    var currentIds = await dbContext.Products
			    .Select(p => p.Id)
			    .Take(100)
			    .ToListAsync();

		    // Create a desired state with some existing and some new products
		    var desiredState = new List<Product>();

		    // Keep 70% of existing products
		    var idsToKeep = currentIds.Take((int)(currentIds.Count * 0.7)).ToList();

		    foreach (var id in idsToKeep)
		    {
			    desiredState.Add(new Product
			    {
				    Id = id,
				    Name = new Faker().Commerce.ProductName(),
				    Description = new Faker().Lorem.Paragraph(),
				    Price = decimal.Parse(new Faker().Commerce.Price())
			    });
		    }

		    // Add some new products
		    var newProducts = new Faker<Product>()
			    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
			    .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
			    .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
			    .Generate(30);

		    desiredState.AddRange(newProducts);
		    
		    await dbContext.BulkSynchronizeAsync(desiredState);

		    return Results.Ok();
	    });
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
