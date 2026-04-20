using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Products;

namespace ProductService.Infrastructure.Database.Mapping;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(250);
        builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.Description).HasMaxLength(1000);

        builder.Property(p => p.Sku).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Barcode).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Category).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Brand).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Manufacturer).IsRequired().HasMaxLength(100);
        builder.Property(p => p.StockQuantity).IsRequired();
        builder.Property(p => p.Weight).IsRequired().HasColumnType("decimal(10,3)");
        builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt);

        builder.HasIndex(p => p.Sku).IsUnique();
    }
}
