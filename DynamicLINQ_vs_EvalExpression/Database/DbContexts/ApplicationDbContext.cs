using System.Reflection;
using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.DbContexts;

public class ApplicationDbContext : DbContext
{
    public DbSet<Author> Authors { get; set; } = null!;

    public DbSet<Book> Books { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
