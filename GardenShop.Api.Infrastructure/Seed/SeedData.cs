using GardenShop.Domain.Entities;
using GardenShop.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GardenShop.Infrastructure.Seed
{
	public static class SeedData
	{
		public static async Task SeedAsync(IApplicationBuilder app)
		{
			using var scope = app.ApplicationServices.CreateScope();
			GardenShopDbContext db = scope.ServiceProvider.GetRequiredService<GardenShopDbContext>();

			if (await db.Categories.AnyAsync()) return;

			var c1 = new Category { Name = "Seeds" };
			var c2 = new Category { Name = "Tools" };
			var c3 = new Category { Name = "Fertilizers" };

			db.Categories.AddRange(c1, c2, c3);
			await db.SaveChangesAsync();

			db.Products.AddRange(
				new Product { Name = "Tomato Seeds", Description = "Cherry variety", Price = 7.99m, Stock = 120, CategoryId = c1.Id },
				new Product { Name = "Garden Trowel", Description = "Stainless steel", Price = 24.90m, Stock = 35, CategoryId = c2.Id },
				new Product { Name = "Universal Fertilizer 1L", Description = "For indoor and garden plants", Price = 19.50m, Stock = 60, CategoryId = c3.Id }
			);

			await db.SaveChangesAsync();
		}
	}
}
