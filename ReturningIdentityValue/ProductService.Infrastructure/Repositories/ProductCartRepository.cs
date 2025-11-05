using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Products;
using ProductService.Infrastructure.Database;

namespace ProductService.Infrastructure.Repositories;

public class ProductCartRepository(ProductDbContext context) : IProductCartRepository
{
    public async Task<ProductCart?> GetByIdAsync(Guid id)
    {
        return await context.ProductCarts
            .AsNoTracking()
            .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(ProductCart productCart)
    {
        await context.ProductCarts.AddAsync(productCart);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductCart productCart)
    {
        var existingCart = await context.ProductCarts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.Id == productCart.Id);
        
        if (existingCart is null)
        {
            return;
        }
        
        context.ProductCartItems.RemoveRange(existingCart.CartItems);
        
        existingCart.CartItems = productCart.CartItems;
        
        context.ProductCarts.Update(existingCart);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var productCart = await context.ProductCarts.FindAsync(id);
        if (productCart is not null)
        {
            context.ProductCarts.Remove(productCart);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ProductCart>> GetByUserIdAsync(int userId)
    {
        return await context.ProductCarts
            .AsNoTracking()
            .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> AddProductToCartAsync(Guid cartId, int productId, int quantity)
    {
        var productCart = await context.ProductCarts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.Id == cartId);

        if (productCart == null)
        {
            return false;
        }

        var existingCartItem = productCart.CartItems
            .FirstOrDefault(x => x.ProductId == productId);

        if (existingCartItem is not null)
        {
            existingCartItem.Quantity += quantity;
            await context.SaveChangesAsync();
            return true;
        }

        var newCartItem = new ProductCartItem
        {
            ProductCartId = cartId,
            ProductId = productId,
            Quantity = quantity
        };
        
        productCart.CartItems.Add(newCartItem);
        
        await context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> UpdateProductInCartAsync(Guid cartId, int productId, int newQuantity)
    {
        var productCart = await context.ProductCarts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.Id == cartId);

        if (productCart is null)
        {
            return false;
        }

        var existingCartItem = productCart.CartItems
            .FirstOrDefault(x => x.ProductId == productId);

        if (existingCartItem is null)
        {
            return false;
        }

        existingCartItem.Quantity = newQuantity;
        
        await context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> RemoveProductFromCartAsync(Guid cartId, int productId)
    {
        var productCart = await context.ProductCarts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.Id == cartId);

        if (productCart is null)
        {
            return false;
        }

        var cartItemToRemove = productCart.CartItems
            .FirstOrDefault(x => x.ProductId == productId);

        if (cartItemToRemove is null)
        {
            return false;
        }

        productCart.CartItems.Remove(cartItemToRemove);

        await context.SaveChangesAsync();
        return true;
    }
}