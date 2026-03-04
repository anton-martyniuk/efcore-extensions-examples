using System.Reflection;
using BulkReadEfCoreExtensions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BulkReadEfCoreExtensions.Infrastructure.Persistence;

public class ShippingDbContext(DbContextOptions<ShippingDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductItem> ProductItems => Set<ProductItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
	    modelBuilder.HasDefaultSchema("products_bulk_read");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
