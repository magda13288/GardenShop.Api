using GardenShop.Application.Abstractions.IServices;
using GardenShop.Application.Services;

namespace GardenShop.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddScoped<ICategoryService, CategoryService>();
			services.AddScoped<IProductService, ProductService>();
			services.AddScoped<IOrderService, OrderService>();
			return services;
		}
	}
}
