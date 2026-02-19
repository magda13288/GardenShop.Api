using GardenShop.Application.Abstractions.Persistence;
using GardenShop.Infrastructure.Persistence;
using GardenShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GardenShop.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dbName = "GardenShop")
		{
			services.AddDbContext<GardenShopDbContext>(o => o.UseInMemoryDatabase(dbName));

			services.AddScoped<IUnitOfWork, UnitOfWork>();

			services.AddScoped<ICategoryRepository, CategoryRepository>();
			services.AddScoped<IProductRepository, ProductRepository>();
			services.AddScoped<IOrderRepository, OrderRepository>();

			return services;
		}
	}
}
