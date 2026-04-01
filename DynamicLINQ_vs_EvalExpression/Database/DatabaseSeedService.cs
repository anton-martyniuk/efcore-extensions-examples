using Bogus;
using Database.DbContexts;
using Database.Entities;

namespace Database;

public static class DatabaseSeedService
{
    public static async Task SeedAsync(ApplicationDbContext dbContext)
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var authors = GetAuthors(3);
        var books = GetBooks(20, authors);

        await dbContext.Authors.AddRangeAsync(authors);
        await dbContext.Books.AddRangeAsync(books);
        await dbContext.SaveChangesAsync();
    }

    private static List<Author> GetAuthors(int count)
    {
        return new Faker<Author>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .Generate(count);
    }

    private static List<Book> GetBooks(int count, List<Author> authors)
    {
        var genres = new[] { "Fiction", "Science", "History", "Fantasy", "Biography", "Technology" };
        var languages = new[] { "English", "French", "German", "Spanish" };
        var publishers = new[] { "Penguin", "HarperCollins", "Oxford Press", "MIT Press", "Springer" };
        var formats = new[] { "Hardcover", "Paperback", "E-Book" };

        return new Faker<Book>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Title, f => f.Commerce.ProductName())
            .RuleFor(x => x.Genre, f => f.PickRandom(genres))
            .RuleFor(x => x.Language, f => f.PickRandom(languages))
            .RuleFor(x => x.Publisher, f => f.PickRandom(publishers))
            .RuleFor(x => x.Format, f => f.PickRandom(formats))
            .RuleFor(x => x.Year, f => f.Random.Int(2000, 2025))
            .RuleFor(x => x.Price, f => Math.Round(f.Random.Decimal(5, 120), 2))
            .RuleFor(x => x.PageCount, f => f.Random.Int(80, 1200))
            .RuleFor(x => x.Rating, f => Math.Round(f.Random.Double(1.0, 5.0), 1))
            .RuleFor(x => x.IsAvailable, f => f.Random.Bool(0.8f))
            .RuleFor(x => x.Author, f => f.PickRandom(authors))
            .Generate(count);
    }
}
