using BulkReadEfCoreExtensions.Infrastructure.Persistence;
using BulkReadEfCoreExtensions.WebApi.Models;

namespace BulkReadEfCoreExtensions.WebApi.Features;

public static class BulkReadEndpoints
{
    public static IEndpointRouteBuilder MapBulkReadEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/products/bulk-read", (ShippingDbContext dbContext, List<ProductInRequest> input) =>
        {
            // Retrieve matching products from the database immediately
            var products = dbContext.Products.BulkRead(input);

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
        .WithName("BulkReadProducts");

        return app;
    }
}
