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
