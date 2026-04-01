using Bogus;
using Database;
using Database.DbContexts;
using EvalExpression.WebApi.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Randomizer.Seed = new Random(1283);

var connectionString = builder.Configuration.GetConnectionString("Sqlite");
builder.Services.AddDbContext<ApplicationDbContext>(x => x.EnableSensitiveDataLogging()
        .UseSqlite(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DatabaseSeedService.SeedAsync(dbContext);

    var result = await dbContext.Books
        .Include(x => x.Author)
        .OrderByDescendingDynamic(x => "x.Author.Name")
        .OrderByDescendingDynamic("x => x.Author.Name")
        .ToListAsync();

    var result2 = await dbContext.Books
        .WhereDynamic(x => "x.Year >= Year", new
        {
            Year = 2022
        })
        .ToListAsync();
    
    var dictionary = new Dictionary<string, object>
    {
        {"Year", 2022}
    };

    var result3 = await dbContext.Books
        .WhereDynamic(x => "x.Year >= Year", dictionary)
        .ToListAsync();
}

app.MapFilterEndpoint();
app.MapDynamicFilterEndpoint();

app.Run();
