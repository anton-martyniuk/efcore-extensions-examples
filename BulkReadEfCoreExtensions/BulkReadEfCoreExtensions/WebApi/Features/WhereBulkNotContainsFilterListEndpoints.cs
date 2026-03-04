using BulkReadEfCoreExtensions.Infrastructure.Persistence;
using BulkReadEfCoreExtensions.WebApi.Models;

namespace BulkReadEfCoreExtensions.WebApi.Features;

public static class WhereBulkNotContainsFilterListEndpoints
{
    public static IEndpointRouteBuilder MapWhereBulkNotContainsFilterListEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/products/filter-list/not-existing", (ShippingDbContext dbContext, List<ProductInRequest> input) =>
        {
            // Returns items from deserializedProducts that don't exist in the database
            var notExistingProducts = dbContext.Products
                .WhereBulkNotContainsFilterList(input, x => x.Id)
                .ToList();

            var response = notExistingProducts.Select(p => new ProductInResponse(p.Id, p.ProductCode, p.SupplierCode));
            return Results.Ok(response);
        })
        .WithName("WhereBulkNotContainsFilterListNotExisting");

        return app;
    }
}
