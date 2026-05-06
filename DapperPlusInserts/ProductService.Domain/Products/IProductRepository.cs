namespace ProductService.Domain.Products;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    
    Task AddAsync(Product product);
    
    Task UpdateAsync(Product product);
    
    Task DeleteAsync(int id);

    Task AddWithDapperPlusAsync(Product product);
    
    Task UpdateWithDapperPlusAsync(Product product);
    
    Task DeleteWithDapperPlusAsync(Product product);
    
    Task MergeWithDapperPlusAsync(Product product);
    
    Task SynchronizeWithDapperPlusAsync(Product product);
}