using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Products;

namespace ProductService.Infrastructure.Database.Mapping;

public class ProductCartItemConfiguration : IEntityTypeConfiguration<ProductCartItem>
{
    public void Configure(EntityTypeBuilder<ProductCartItem> builder)
    {
        builder.HasKey(pci => pci.Id);

        builder.Property(pci => pci.Id)
            .ValueGeneratedOnAdd();

        builder.HasOne(pci => pci.ProductCart)
            .WithMany(pc => pc.CartItems)
            .HasForeignKey(pci => pci.ProductCartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pci => pci.Product)
            .WithMany()
            .HasForeignKey(pci => pci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(pci => pci.Quantity)
            .IsRequired()
            .HasDefaultValue(1);
    }
}