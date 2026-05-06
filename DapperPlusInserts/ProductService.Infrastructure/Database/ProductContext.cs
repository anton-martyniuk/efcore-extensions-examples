using System.Data;
using ProductService.Domain.Products;
using Z.Dapper.Plus;

namespace ProductService.Infrastructure.Database;

public class ProductContext : DapperPlusContext
{
    public ProductContext(IDbConnection connection) : base(connection)
    {
        Entity<Product>()
            .Table($"{DatabaseConsts.Schema}.{DatabaseConsts.ProductsTable}")
            .Map(x => x.StockQuantity, "stock_quantity")
            .Map(x => x.CreatedAt, "created_at")
            .Map(x => x.UpdatedAt, "updated_at")
            .Map(x => x.IsActive, "is_active")
            .AutoMap();
    }
}