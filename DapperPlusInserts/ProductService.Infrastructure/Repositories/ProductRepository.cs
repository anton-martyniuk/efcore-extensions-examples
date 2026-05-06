using Dapper;
using ProductService.Domain.Products;
using ProductService.Infrastructure.Database;
using Z.Dapper.Plus;

namespace ProductService.Infrastructure.Repositories;

public class ProductRepository(IConnectionFactory connectionFactory) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT id, name, price, description, sku, barcode, category, brand, manufacturer,
                   stock_quantity, weight, is_active, created_at, updated_at
            FROM products_identity.products
            WHERE id = @id";

        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { id });
    }

    public async Task AddAsync(Product product)
    {
        const string sql = @"
            INSERT INTO products_identity.products
                (name, price, description, sku, barcode, category, brand, manufacturer,
                 stock_quantity, weight, is_active, created_at, updated_at)
            VALUES
                (@Name, @Price, @Description, @Sku, @Barcode, @Category, @Brand, @Manufacturer,
                 @StockQuantity, @Weight, @IsActive, @CreatedAt, @UpdatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = connectionFactory.CreateConnection();
        product.Id = await connection.ExecuteScalarAsync<int>(sql, product);
    }

    public async Task AddWithDapperPlusAsync(Product product)
    {
        using var connection = connectionFactory.CreateConnection();
        
        // var context = new ProductContext(connection);
        // await context.SingleInsertAsync(product);
        // await connection.SingleInsertAsync(context, product);
        
        await connection.SingleInsertAsync(product);
    }
    
    public async Task UpdateWithDapperPlusAsync(Product product)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.SingleUpdateAsync(product);
    }
    
    public async Task DeleteWithDapperPlusAsync(Product product)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.SingleDeleteAsync(product);
    }
    
    public async Task MergeWithDapperPlusAsync(Product product)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.SingleMergeAsync(product);
    }
    
    public async Task SynchronizeWithDapperPlusAsync(Product product)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.SingleSynchronizeAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        const string sql = @"
            UPDATE products_identity.products
            SET name = @Name,
                price = @Price,
                description = @Description,
                sku = @Sku,
                barcode = @Barcode,
                category = @Category,
                brand = @Brand,
                manufacturer = @Manufacturer,
                stock_quantity = @StockQuantity,
                weight = @Weight,
                is_active = @IsActive,
                created_at = @CreatedAt,
                updated_at = @UpdatedAt
            WHERE id = @Id";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, product);
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM products_identity.products WHERE id = @id";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { id });
    }
}
