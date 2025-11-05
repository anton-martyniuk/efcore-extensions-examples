using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Products;
using ProductService.Domain.Users;
using ProductService.Infrastructure.Database.Mapping;

namespace ProductService.Infrastructure.Database;

public class ProductDbContext(
	DbContextOptions<ProductDbContext> options)
	: DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCart> ProductCarts { get; set; }

    public DbSet<ProductCartItem> ProductCartItems { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(DatabaseConsts.Schema);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCartConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCartItemConfiguration());
    }
}
