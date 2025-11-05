using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Products;
using ProductService.Infrastructure.Database;
using ProductService.Infrastructure.Repositories;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class HostDiExtensions
{
	public static IServiceCollection AddWebHostInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<IProductRepository, ProductRepository>();
		services.AddScoped<IProductCartRepository, ProductCartRepository>();

		services.AddEfCore(configuration);

		return services;
	}

	private static IServiceCollection AddEfCore(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("SqlServer");

		services.AddDbContext<ProductDbContext>((_, options) =>
		{
			options.EnableSensitiveDataLogging()
				.UseSqlServer(connectionString, npgsqlOptions =>
				{
					npgsqlOptions.MigrationsHistoryTable(DatabaseConsts.MigrationHistoryTable, DatabaseConsts.Schema);
				});

			options.UseSnakeCaseNamingConvention();
		});

		return services;
	}
}
