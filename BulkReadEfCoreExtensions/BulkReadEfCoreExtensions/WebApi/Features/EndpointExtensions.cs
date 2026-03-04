namespace BulkReadEfCoreExtensions.WebApi.Features;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapFeatures(this IEndpointRouteBuilder app)
    {
        // Place to register Minimal API endpoints grouped by feature.
        // Upcoming endpoints for EF Core bulk data retrieval examples will be added here.
        app.MapWhereBulkContainsEndpoints();
        app.MapWhereBulkNotContainsEndpoints();
        app.MapBulkReadEndpoints();
        app.MapWhereBulkContainsFilterListEndpoints();
        app.MapWhereBulkNotContainsFilterListEndpoints();
        return app;
    }
}
