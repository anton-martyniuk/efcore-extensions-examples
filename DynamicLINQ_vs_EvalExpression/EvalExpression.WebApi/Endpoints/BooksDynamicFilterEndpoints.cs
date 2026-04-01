using Database.DbContexts;
using EvalExpression.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EvalExpression.WebApi.Endpoints;

public static class BooksDynamicFilterEndpoints
{
    public static void MapDynamicFilterEndpoint(this WebApplication app)
    {
        app.MapPost("/api/books", async (
                ApplicationDbContext dbContext,
                BookFilterRequest request) =>
        {
            var query = dbContext.Books
                .Include(x => x.Author)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Filter))
            {
                query = query.WhereDynamic(request.Filter);
            }
            
            if (!string.IsNullOrWhiteSpace(request.Sort))
            {
                query = query.OrderByDynamic(request.Sort);
            }

            var books = await query.ToListAsync();
            return Results.Ok(books);
        });
    }
}
