namespace ProductService.Domain.Products;

public class ProductCartItem
{
    public int Id { get; set; }

    public int ProductCartId { get; set; }
    public ProductCart ProductCart { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
}