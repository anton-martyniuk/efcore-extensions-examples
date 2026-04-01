using Database.DbContexts;
using EvalExpression.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EvalExpression.WebApi.Endpoints;

public static class BooksStructuredFilterEndpoints
{
    public static void MapFilterEndpoint(this WebApplication app)
    {
        app.MapPost("/api/books/filter", async (
                ApplicationDbContext dbContext,
                BookStructuredFilterRequest request) =>
        {
            var query = dbContext.Books
                .Include(x => x.Author)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                query = query.Where(b => b.Title.Contains(request.Title));
            }

            if (!string.IsNullOrWhiteSpace(request.Genre))
            {
                query = query.Where(b => b.Genre == request.Genre);
            }

            if (!string.IsNullOrWhiteSpace(request.Language))
            {
                query = query.Where(b => b.Language == request.Language);
            }

            if (!string.IsNullOrWhiteSpace(request.Publisher))
            {
                query = query.Where(b => b.Publisher == request.Publisher);
            }

            if (!string.IsNullOrWhiteSpace(request.Format))
            {
                query = query.Where(b => b.Format == request.Format);
            }

            if (request.YearFrom.HasValue)
            {
                query = query.Where(b => b.Year >= request.YearFrom.Value);
            }

            if (request.YearTo.HasValue)
            {
                query = query.Where(b => b.Year <= request.YearTo.Value);
            }

            if (request.PriceFrom.HasValue)
            {
                query = query.Where(b => b.Price >= request.PriceFrom.Value);
            }

            if (request.PriceTo.HasValue)
            {
                query = query.Where(b => b.Price <= request.PriceTo.Value);
            }

            if (request.PageCountFrom.HasValue)
            {
                query = query.Where(b => b.PageCount >= request.PageCountFrom.Value);
            }

            if (request.PageCountTo.HasValue)
            {
                query = query.Where(b => b.PageCount <= request.PageCountTo.Value);
            }

            if (request.MinRating.HasValue)
            {
                query = query.Where(b => b.Rating >= request.MinRating.Value);
            }

            if (request.IsAvailable.HasValue)
            {
                query = query.Where(b => b.IsAvailable == request.IsAvailable.Value);
            }

            var isDescending = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                query = request.SortBy.ToLower() switch
                {
                    "title"     => isDescending ? query.OrderByDescending(b => b.Title)     : query.OrderBy(b => b.Title),
                    "year"      => isDescending ? query.OrderByDescending(b => b.Year)      : query.OrderBy(b => b.Year),
                    "price"     => isDescending ? query.OrderByDescending(b => b.Price)     : query.OrderBy(b => b.Price),
                    "rating"    => isDescending ? query.OrderByDescending(b => b.Rating)    : query.OrderBy(b => b.Rating),
                    "pagecount" => isDescending ? query.OrderByDescending(b => b.PageCount) : query.OrderBy(b => b.PageCount),
                    _           => query
                };
            }

            var books = await query.ToListAsync();
            return Results.Ok(books);
        });
    }
}
