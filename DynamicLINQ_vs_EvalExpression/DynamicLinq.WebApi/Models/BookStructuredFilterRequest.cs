namespace DynamicLinq.WebApi.Models;

public class BookStructuredFilterRequest
{
    public string? Title { get; set; }

    public string? Genre { get; set; }

    public string? Language { get; set; }

    public string? Publisher { get; set; }

    public string? Format { get; set; }

    public int? YearFrom { get; set; }

    public int? YearTo { get; set; }

    public decimal? PriceFrom { get; set; }

    public decimal? PriceTo { get; set; }

    public int? PageCountFrom { get; set; }

    public int? PageCountTo { get; set; }

    public double? MinRating { get; set; }

    public bool? IsAvailable { get; set; }

    // Field name to sort by: "title", "year", "price", "rating", "pagecount"
    public string? SortBy { get; set; }

    // "asc" or "desc" (defaults to "asc")
    public string? SortDirection { get; set; }
}
