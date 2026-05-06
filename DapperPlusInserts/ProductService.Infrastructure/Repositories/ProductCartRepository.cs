using System.Data.Common;
using Dapper;
using ProductService.Domain.Products;
using ProductService.Domain.Users;
using ProductService.Infrastructure.Database;

namespace ProductService.Infrastructure.Repositories;

public class ProductCartRepository(IConnectionFactory connectionFactory) : IProductCartRepository
{
    private const string CartColumns =
        "pc.id, pc.quantity, pc.user_id, pc.created_on";

    private const string ItemColumns =
        "ci.id, ci.product_cart_id, ci.product_id, ci.quantity";

    private const string ProductColumns =
        "p.id, p.name, p.price, p.description, p.sku, p.barcode, p.category, p.brand, p.manufacturer, " +
        "p.stock_quantity, p.weight, p.is_active, p.created_at, p.updated_at";

    private const string UserColumns =
        "u.id, u.username, u.email";

    public async Task<ProductCart?> GetByIdAsync(int id)
    {
        var sql = $@"
            SELECT {CartColumns}, {ItemColumns}, {ProductColumns}, {UserColumns}
            FROM products_identity.product_carts pc
            INNER JOIN products_identity.users u ON u.id = pc.user_id
            LEFT JOIN products_identity.product_cart_items ci ON ci.product_cart_id = pc.id
            LEFT JOIN products_identity.products p ON p.id = ci.product_id
            WHERE pc.id = @id";

        using var connection = connectionFactory.CreateConnection();
        var carts = await QueryGraphAsync(connection, sql, new { id });
        return carts.FirstOrDefault();
    }

    public async Task<IEnumerable<ProductCart>> GetByUserIdAsync(int userId)
    {
        var sql = $@"
            SELECT {CartColumns}, {ItemColumns}, {ProductColumns}, {UserColumns}
            FROM products_identity.product_carts pc
            INNER JOIN products_identity.users u ON u.id = pc.user_id
            LEFT JOIN products_identity.product_cart_items ci ON ci.product_cart_id = pc.id
            LEFT JOIN products_identity.products p ON p.id = ci.product_id
            WHERE pc.user_id = @userId";

        using var connection = connectionFactory.CreateConnection();
        return await QueryGraphAsync(connection, sql, new { userId });
    }

    private static async Task<List<ProductCart>> QueryGraphAsync(
        System.Data.IDbConnection connection,
        string sql,
        object parameters)
    {
        var lookup = new Dictionary<int, ProductCart>();

        await connection.QueryAsync<ProductCart, ProductCartItem, Product, User, ProductCart>(
            sql,
            (cart, item, product, user) =>
            {
                if (!lookup.TryGetValue(cart.Id, out var existing))
                {
                    existing = cart;
                    existing.User = user;
                    existing.CartItems = new List<ProductCartItem>();
                    lookup.Add(cart.Id, existing);
                }

                if (item is not null && item.Id != 0)
                {
                    item.Product = product;
                    existing.CartItems.Add(item);
                }

                return existing;
            },
            parameters,
            splitOn: "id,id,id");

        return lookup.Values.ToList();
    }

    public async Task AddAsync(ProductCart productCart)
    {
        const string insertCartSql = @"
            INSERT INTO products_identity.product_carts (quantity, user_id, created_on)
            VALUES (@Quantity, @UserId, @CreatedOn);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        const string insertItemSql = @"
            INSERT INTO products_identity.product_cart_items (product_cart_id, product_id, quantity)
            VALUES (@ProductCartId, @ProductId, @Quantity)";

        await using var connection = (DbConnection)connectionFactory.CreateConnection();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var cartId = await connection.ExecuteScalarAsync<int>(insertCartSql, productCart, transaction);
            productCart.Id = cartId;

            if (productCart.CartItems.Count > 0)
            {
                foreach (var item in productCart.CartItems)
                {
                    item.ProductCartId = cartId;
                }

                await connection.ExecuteAsync(insertItemSql, productCart.CartItems, transaction);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateAsync(ProductCart productCart)
    {
        const string existsSql =
            "SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM products_identity.product_carts WHERE id = @id) THEN 1 ELSE 0 END AS BIT)";

        const string deleteItemsSql =
            "DELETE FROM products_identity.product_cart_items WHERE product_cart_id = @id";

        const string insertItemSql = @"
            INSERT INTO products_identity.product_cart_items (product_cart_id, product_id, quantity)
            VALUES (@ProductCartId, @ProductId, @Quantity)";

        await using var connection = (DbConnection)connectionFactory.CreateConnection();

        var exists = await connection.ExecuteScalarAsync<bool>(existsSql, new { id = productCart.Id });
        if (!exists)
        {
            return;
        }

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            await connection.ExecuteAsync(deleteItemsSql, new { id = productCart.Id }, transaction);

            if (productCart.CartItems.Count > 0)
            {
                foreach (var item in productCart.CartItems)
                {
                    item.ProductCartId = productCart.Id;
                    item.Id = 0;
                }

                await connection.ExecuteAsync(insertItemSql, productCart.CartItems, transaction);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM products_identity.product_carts WHERE id = @id";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { id });
    }

    public async Task<bool> AddProductToCartAsync(int cartId, int productId, int quantity)
    {
        const string cartExistsSql =
            "SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM products_identity.product_carts WHERE id = @cartId) THEN 1 ELSE 0 END AS BIT)";

        const string upsertSql = @"
            UPDATE products_identity.product_cart_items
            SET quantity = quantity + @quantity
            WHERE product_cart_id = @cartId AND product_id = @productId;

            IF @@ROWCOUNT = 0
            BEGIN
                INSERT INTO products_identity.product_cart_items (product_cart_id, product_id, quantity)
                VALUES (@cartId, @productId, @quantity);
            END";

        using var connection = connectionFactory.CreateConnection();
        var exists = await connection.ExecuteScalarAsync<bool>(cartExistsSql, new { cartId });
        if (!exists)
        {
            return false;
        }

        await connection.ExecuteAsync(upsertSql, new { cartId, productId, quantity });
        return true;
    }

    public async Task<bool> UpdateProductInCartAsync(int cartId, int productId, int newQuantity)
    {
        const string sql = @"
            UPDATE products_identity.product_cart_items
            SET quantity = @newQuantity
            WHERE product_cart_id = @cartId AND product_id = @productId";

        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { cartId, productId, newQuantity });
        return rows > 0;
    }

    public async Task<bool> RemoveProductFromCartAsync(int cartId, int productId)
    {
        const string sql = @"
            DELETE FROM products_identity.product_cart_items
            WHERE product_cart_id = @cartId AND product_id = @productId";

        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { cartId, productId });
        return rows > 0;
    }
}
