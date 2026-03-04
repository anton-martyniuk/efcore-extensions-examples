using BulkReadEfCoreExtensions.Infrastructure.Persistence;
using BulkReadEfCoreExtensions.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkReadEfCoreExtensions.WebApi.Features;

public static class WhereBulkNotContainsEndpoints
{
    public static IEndpointRouteBuilder MapWhereBulkNotContainsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/products/where-bulk-not-contains", async (ShippingDbContext dbContext) =>
        {
            // @nuget: Z.EntityFramework.Extensions.EFCore
            var discontinuedProductIds = await GetDiscontinuedProductIdsFromExternalSystem(dbContext); // Returns 20,000 IDs

            var activeProducts = await dbContext.Products
	            .Include(product => product.Category!)
	            .WhereBulkNotContains(discontinuedProductIds, x => x.Id)
	            .ToListAsync();

            var response = activeProducts.Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.ProductCode,
                p.SupplierCode,
                p.Price,
                p.Stock,
                p.IsActive,
                p.Category!.Name
            ));

            return Results.Ok(response);
        })
        .WithName("WhereBulkNotContainsByIds");

        return app;
    }

    private static async Task<List<int>> GetDiscontinuedProductIdsFromExternalSystem(ShippingDbContext dbContext)
    {
        // Simulate external system by selecting a subset of existing IDs
        return await dbContext.Products
            .OrderByDescending(p => p.Id)
            .Select(p => p.Id)
            .Take(2000)
            .ToListAsync();
    }
}
