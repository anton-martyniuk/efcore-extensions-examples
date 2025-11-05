using Bogus;
using Carter;
using Microsoft.EntityFrameworkCore;
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

		    await dbContext.BulkInsertAsync(products);

		    return Results.Ok();
	    });
	    app.MapPost("/products/efcore-bulk-insert-optimized", async (ProductDbContext dbContext) =>
	    {
		    var products = GenerateProducts(10_000);

		    await dbContext.BulkInsertOptimizedAsync(products);

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
