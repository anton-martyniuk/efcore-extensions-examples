using BulkReadEfCoreExtensions.Infrastructure.Persistence;
using BulkReadEfCoreExtensions.WebApi.Models;

namespace BulkReadEfCoreExtensions.WebApi.Features;

public static class WhereBulkContainsFilterListEndpoints
{
    public static IEndpointRouteBuilder MapWhereBulkContainsFilterListEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/products/filter-list/existing", (ShippingDbContext dbContext, List<ProductInRequest> input) =>
        {
            // Returns items from deserializedProducts that exist in the database
            var existingProducts = dbContext.Products
                .WhereBulkContainsFilterList(input, x => x.Id)
                .ToList();

            var response = existingProducts.Select(p => new ProductInResponse(p.Id, p.ProductCode, p.SupplierCode));
            return Results.Ok(response);
        })
        .WithName("WhereBulkContainsFilterListExisting");

        return app;
    }
}
