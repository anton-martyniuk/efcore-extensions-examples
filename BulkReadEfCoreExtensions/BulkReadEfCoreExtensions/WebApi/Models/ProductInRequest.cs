namespace BulkReadEfCoreExtensions.WebApi.Models;

// Minimal shape used by BulkRead/FilterList examples; properties names align with entity
public readonly record struct ProductInRequest(
    int Id,
    string? ProductCode,
    string? SupplierCode
);
