using BulkReadEfCoreExtensions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BulkReadEfCoreExtensions.Infrastructure.Persistence.Configurations;

public class ProductItemConfiguration : IEntityTypeConfiguration<ProductItem>
{
    public void Configure(EntityTypeBuilder<ProductItem> builder)
    {
        builder.ToTable("ProductItems");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Sku)
            .HasMaxLength(64)
            .IsRequired();
    }
}
