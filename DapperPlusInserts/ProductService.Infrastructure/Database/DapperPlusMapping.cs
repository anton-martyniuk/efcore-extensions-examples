using ProductService.Domain.Products;
using ProductService.Domain.Users;
using Z.Dapper.Plus;

namespace ProductService.Infrastructure.Database;

public static class DapperPlusMapping
{
    public static void Map()
    {
        // Global level
        DapperPlusManager.DefaultContext.Entity<Product>();
        
        DapperPlusManager.Entity<Product>()
            .Table($"{DatabaseConsts.Schema}.{DatabaseConsts.ProductsTable}")
            .Map(x => x.StockQuantity, "stock_quantity")
            .Map(x => x.CreatedAt, "created_at")
            .Map(x => x.UpdatedAt, "updated_at")
            .Map(x => x.IsActive, "is_active")
            .AutoMap();
        
        DapperPlusManager.Entity<User>()
            .Table($"{DatabaseConsts.Schema}.{DatabaseConsts.UsersTable}")
            .Identity(x => x.Id, "id")
            .Map(x => x.Username, "username")
            .Map(x => x.Email, "email");

        DapperPlusManager.Entity<ProductCart>()
            .Table($"{DatabaseConsts.Schema}.{DatabaseConsts.ProductCartsTable}")
            .Identity(x => x.Id, "id")
            .Map(x => x.Quantity, "quantity")
            .Map(x => x.UserId, "user_id")
            .Map(x => x.CreatedOn, "created_on")
            .Ignore(x => x.User)
            .Ignore(x => x.CartItems);

        DapperPlusManager.Entity<ProductCartItem>()
            .Table($"{DatabaseConsts.Schema}.{DatabaseConsts.ProductCartItemsTable}")
            .Identity(x => x.Id, "id")
            .Map(x => x.ProductCartId, "product_cart_id")
            .Map(x => x.ProductId, "product_id")
            .Map(x => x.Quantity, "quantity")
            .Ignore(x => x.ProductCart)
            .Ignore(x => x.Product);
    }
}