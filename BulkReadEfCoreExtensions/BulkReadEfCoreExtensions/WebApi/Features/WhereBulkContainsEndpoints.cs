using BulkReadEfCoreExtensions.Infrastructure.Persistence;
using BulkReadEfCoreExtensions.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkReadEfCoreExtensions.WebApi.Features;

public static class WhereBulkContainsEndpoints
{
    public static IEndpointRouteBuilder MapWhereBulkContainsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/products/where-bulk-contains", async (ShippingDbContext dbContext) =>
        {
            var productIds = await GetProductIdsFromExternalSystem(dbContext);

            var products = await dbContext.Products
	            .Include(product => product.Category)
	            .WhereBulkContains(productIds, x => x.Id)
	            .ToListAsync();

            var response = products.Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.ProductCode,
                p.SupplierCode,
                p.Price,
                p.Stock,
                p.IsActive,
                p.Category?.Name
            ));

            return Results.Ok(response);
        })
        .WithName("WhereBulkContainsByIds");

        return app;
    }

    private static async Task<List<int>> GetProductIdsFromExternalSystem(ShippingDbContext dbContext)
    {
        // Simulate external system by taking first 5000 product ids from database
        return await dbContext.Products
            .OrderBy(p => p.Id)
            .Select(p => p.Id)
            .Take(5000)
            .ToListAsync();
    }
}
