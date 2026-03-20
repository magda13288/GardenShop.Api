using GardenShop.Application.Abstractions.IServices;
using GardenShop.Application.Services;
using Microsoft.Extensions.DependencyInjection;

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
