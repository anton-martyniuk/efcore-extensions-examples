namespace ProductService.Domain.Products;

public interface IProductCartRepository
{
    Task<ProductCart?> GetByIdAsync(Guid id);

    Task AddAsync(ProductCart productCart);

    Task UpdateAsync(ProductCart productCart);

    Task DeleteAsync(Guid id);

    Task<IEnumerable<ProductCart>> GetByUserIdAsync(int userId);
    
    Task<bool> AddProductToCartAsync(Guid cartId, int productId, int quantity);

    Task<bool> UpdateProductInCartAsync(Guid cartId, int productId, int newQuantity);

    Task<bool> RemoveProductFromCartAsync(Guid cartId, int productId);
}