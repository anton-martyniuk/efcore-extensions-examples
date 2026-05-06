using System.Data.Common;
using Bogus;
using Carter;
using Dapper;
using ProductService.Domain.Products;
using ProductService.Domain.Users;
using ProductService.Infrastructure.Database;

namespace ProductService.Host.Features.Products;

public class ProductCartsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/product-carts/dapper-insert-graph", async (IConnectionFactory connectionFactory) =>
        {
            var (_, _, carts) = await GenerateAndPersistDependenciesAsync(connectionFactory, cartCount: 10_000);

            const string insertCartSql = @"
                INSERT INTO products_identity.product_carts (quantity, user_id, created_on)
                OUTPUT INSERTED.id
                VALUES (@Quantity, @UserId, @CreatedOn);";

            const string insertItemSql = @"
                INSERT INTO products_identity.product_cart_items (product_cart_id, product_id, quantity)
                VALUES (@ProductCartId, @ProductId, @Quantity)";

            await using var connection = (DbConnection)connectionFactory.CreateConnection();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                foreach (var cart in carts)
                {
                    cart.UserId = cart.User.Id;
                    var newId = await connection.ExecuteScalarAsync<int>(insertCartSql, cart, transaction);
                    cart.Id = newId;

                    foreach (var item in cart.CartItems)
                    {
                        item.ProductCartId = newId;
                        item.ProductId = item.Product.Id;
                    }

                    if (cart.CartItems.Count > 0)
                    {
                        await connection.ExecuteAsync(insertItemSql, cart.CartItems, transaction);
                    }
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return Results.Ok();
        });
    }

    private static async Task<(List<User> users, List<Product> products, List<ProductCart> carts)> GenerateAndPersistDependenciesAsync(
        IConnectionFactory connectionFactory,
        int cartCount)
    {
        var users = GenerateUsers(100);
        var products = GenerateProducts(200);

        const string insertUserSql = @"
            INSERT INTO products_identity.users (username, email)
            OUTPUT INSERTED.id
            VALUES (@Username, @Email);";

        const string insertProductSql = @"
            INSERT INTO products_identity.products
                (name, price, description, sku, barcode, category, brand, manufacturer,
                 stock_quantity, weight, is_active, created_at, updated_at)
            OUTPUT INSERTED.id
            VALUES
                (@Name, @Price, @Description, @Sku, @Barcode, @Category, @Brand, @Manufacturer,
                 @StockQuantity, @Weight, @IsActive, @CreatedAt, @UpdatedAt);";

        await using var connection = (DbConnection)connectionFactory.CreateConnection();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            foreach (var user in users)
            {
                user.Id = await connection.ExecuteScalarAsync<int>(insertUserSql, user, transaction);
            }

            foreach (var product in products)
            {
                product.Id = await connection.ExecuteScalarAsync<int>(insertProductSql, product, transaction);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        var carts = GenerateProductCarts(cartCount, users, products);
        return (users, products, carts);
    }

    private static List<ProductCart> GenerateProductCarts(int count, List<User> users, List<Product> products)
    {
        return new Faker<ProductCart>()
            .RuleFor(pc => pc.Quantity, f => f.Random.Int(1, 10))
            .RuleFor(pc => pc.User, f => f.PickRandom(users))
            .RuleFor(pc => pc.CreatedOn, f => f.Date.Recent(30))
            .RuleFor(pc => pc.CartItems, (f, _) =>
            {
                var cartItems = new List<ProductCartItem>();
                var itemCount = f.Random.Int(1, 5);

                for (var i = 0; i < itemCount; i++)
                {
                    var product = f.PickRandom(products);
                    cartItems.Add(new ProductCartItem
                    {
                        Product = product,
                        ProductId = product.Id,
                        Quantity = f.Random.Int(1, 5)
                    });
                }

                return cartItems;
            })
            .Generate(count);
    }

    private static List<User> GenerateUsers(int count)
    {
        return new Faker<User>()
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .Generate(count);
    }

    private static List<Product> GenerateProducts(int count)
    {
        return new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
            .RuleFor(p => p.Sku, f => $"SKU-{Guid.NewGuid():N}")
            .RuleFor(p => p.Barcode, f => f.Commerce.Ean8())
            .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
            .RuleFor(p => p.Brand, f => f.Company.CompanyName())
            .RuleFor(p => p.Manufacturer, f => f.Company.CompanyName())
            .RuleFor(p => p.StockQuantity, f => f.Random.Int(0, 10_000))
            .RuleFor(p => p.Weight, f => Math.Round(f.Random.Decimal(0.01m, 50m), 3))
            .RuleFor(p => p.IsActive, f => f.Random.Bool(0.9f))
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(2).ToUniversalTime())
            .RuleFor(p => p.UpdatedAt, f => f.Date.Recent(30).OrNull(f, 0.3f)?.ToUniversalTime())
            .Generate(count);
    }
}
