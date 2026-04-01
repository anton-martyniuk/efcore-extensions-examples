using Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.DbMapping;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Title);

        builder.Property(b => b.Id)
            .HasColumnName("id");

        builder.Property(b => b.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasColumnType("nvarchar(100)");

        builder.Property(b => b.Genre)
            .HasColumnName("genre")
            .HasColumnType("nvarchar(50)");

        builder.Property(b => b.Language)
            .HasColumnName("language")
            .HasColumnType("nvarchar(50)");

        builder.Property(b => b.Publisher)
            .HasColumnName("publisher")
            .HasColumnType("nvarchar(100)");

        builder.Property(b => b.Format)
            .HasColumnName("format")
            .HasColumnType("nvarchar(30)");

        builder.Property(b => b.Year)
            .HasColumnName("year")
            .IsRequired();

        builder.Property(b => b.Price)
            .HasColumnName("price")
            .HasColumnType("decimal(10,2)");

        builder.Property(b => b.PageCount)
            .HasColumnName("page_count");

        builder.Property(b => b.Rating)
            .HasColumnName("rating");

        builder.Property(b => b.IsAvailable)
            .HasColumnName("is_available")
            .IsRequired();

        builder.Property(b => b.AuthorId)
            .HasColumnName("author_id")
            .IsRequired();

        builder.HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .IsRequired();
    }
}
