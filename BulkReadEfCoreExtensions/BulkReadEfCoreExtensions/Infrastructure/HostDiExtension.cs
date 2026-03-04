using BulkReadEfCoreExtensions.Infrastructure.Persistence;
using BulkReadEfCoreExtensions.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;

namespace BulkReadEfCoreExtensions.Infrastructure;

public static class HostDiExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SqlServer")!;

		services.AddDbContext<ShippingDbContext>(options =>
		{
			options.UseSqlServer(connectionString, sqlOptions =>
			{
				sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "products_bulk_read");
			});
		});

        services.AddScoped<SeedService>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
