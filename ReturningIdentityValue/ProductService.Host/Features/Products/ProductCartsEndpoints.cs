using Bogus;
using Carter;
using ProductService.Domain.Products;
using ProductService.Domain.Users;
using ProductService.Infrastructure.Database;

namespace ProductService.Host.Features.Products;

public class ProductCartsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
	    app.MapPost("/product-carts/insert-graph", async (ProductDbContext dbContext) =>
	    {
		    var productCarts = GenerateProductCarts(10_000);

		    dbContext.ProductCarts.AddRange(productCarts);
		    await dbContext.SaveChangesAsync();

		    return Results.Ok();
	    });
	    
	    app.MapPost("/product-carts/bulk-insert-graph", async (ProductDbContext dbContext) =>
	    {
		    var productCarts = GenerateProductCarts(10_000);

		    await dbContext.BulkInsertAsync(productCarts, options => options.IncludeGraph = true);

		    return Results.Ok();
	    });
    }

    private static List<ProductCart> GenerateProductCarts(int count)
    {
	    var users = new Faker<User>()
		    .RuleFor(u => u.Username, f => f.Internet.UserName())
		    .RuleFor(u => u.Email, f => f.Internet.Email())
		    .Generate(100);

	    var products = new Faker<Product>()
		    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
		    .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
		    .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
		    .Generate(200);

	    return new Faker<ProductCart>()
		    .RuleFor(pc => pc.Quantity, f => f.Random.Int(1, 10))
		    .RuleFor(pc => pc.User, f =>  f.PickRandom(users))
		    .RuleFor(pc => pc.CreatedOn, f => f.Date.Recent(30))
		    .RuleFor(pc => pc.CartItems, (f, pc) =>
		    {
			    var cartItems = new List<ProductCartItem>();
			    var itemCount = f.Random.Int(1, 5);

			    for (var i = 0; i < itemCount; i++)
			    {
				    var product = f.PickRandom(products);
				    cartItems.Add(new ProductCartItem
				    {
					    ProductCartId = pc.Id,
					    ProductId = product.Id,
					    Product = product,
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
		    .RuleFor(u => u.Id, f => f.IndexFaker + 1)
		    .RuleFor(u => u.Username, f => f.Internet.UserName())
		    .RuleFor(u => u.Email, f => f.Internet.Email())
		    .Generate(count);
    }

    private static List<Product> GenerateProducts(int count)
    {
	    return new Faker<Product>()
		    .RuleFor(p => p.Id, f => f.IndexFaker + 1)
		    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
		    .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
		    .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
		    .Generate(count);
    }
}
