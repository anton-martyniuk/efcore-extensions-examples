namespace Database.Entities;

public class Book
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }

    public string? Genre { get; set; }

    public string? Language { get; set; }

    public string? Publisher { get; set; }

    public string? Format { get; set; }

    public required int Year { get; set; }

    public decimal? Price { get; set; }

    public int? PageCount { get; set; }

    public double? Rating { get; set; }

    public bool IsAvailable { get; set; } = true;

    public required Guid AuthorId { get; set; }

    public required Author Author { get; set; }
}
