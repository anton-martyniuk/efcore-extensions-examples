namespace ProductService.Domain.Products;

public interface IProductCartRepository
{
    Task<ProductCart?> GetByIdAsync(int id);

    Task AddAsync(ProductCart productCart);

    Task UpdateAsync(ProductCart productCart);

    Task DeleteAsync(int id);

    Task<IEnumerable<ProductCart>> GetByUserIdAsync(int userId);
    
    Task<bool> AddProductToCartAsync(int cartId, int productId, int quantity);

    Task<bool> UpdateProductInCartAsync(int cartId, int productId, int newQuantity);

    Task<bool> RemoveProductFromCartAsync(int cartId, int productId);
}