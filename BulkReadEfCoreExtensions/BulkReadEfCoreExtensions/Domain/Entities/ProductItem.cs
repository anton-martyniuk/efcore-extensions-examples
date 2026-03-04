namespace BulkReadEfCoreExtensions.Domain.Entities;

public class ProductItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
