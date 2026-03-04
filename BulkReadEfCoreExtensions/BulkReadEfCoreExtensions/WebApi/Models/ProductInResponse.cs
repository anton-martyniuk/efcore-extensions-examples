namespace BulkReadEfCoreExtensions.WebApi.Models;

public readonly record struct ProductInResponse(
    int Id,
    string? ProductCode,
    string? SupplierCode
);
