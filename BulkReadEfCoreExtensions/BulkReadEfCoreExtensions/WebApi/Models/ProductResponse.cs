namespace BulkReadEfCoreExtensions.WebApi.Models;

public readonly record struct ProductResponse(
    int Id,
    string Name,
    string ProductCode,
    string SupplierCode,
    decimal Price,
    int Stock,
    bool IsActive,
    string? CategoryName
);
