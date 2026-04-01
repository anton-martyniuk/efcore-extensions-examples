using System.Linq.Dynamic.Core;
using Bogus;
using Database;
using Database.DbContexts;
using DynamicLinq.WebApi.Endpoints;
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
        .Where(x => x.Year > 2020)
        .OrderBy(x => x.Year)
        .ToListAsync();

    var result2 = await dbContext.Books
        .Include(x => x.Author)
        .Where("Year > 2020")
        .OrderBy("Year")
        .ToListAsync();
    
    var result3 = await dbContext.Books
        .Where("Year >= @0 && Price <= @1", 2020, 70)
        .ToListAsync();
}

app.MapFilterEndpoint();

app.Run();