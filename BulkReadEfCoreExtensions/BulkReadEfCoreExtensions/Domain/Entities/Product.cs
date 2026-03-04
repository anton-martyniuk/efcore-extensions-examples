namespace BulkReadEfCoreExtensions.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string SupplierCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<ProductItem> Items { get; set; } = new List<ProductItem>();
}
