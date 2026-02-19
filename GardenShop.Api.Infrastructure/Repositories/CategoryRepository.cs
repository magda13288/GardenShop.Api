using GardenShop.Application.Abstractions.Persistence;
using GardenShop.Domain.Entities;
using GardenShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GardenShop.Infrastructure.Repositories
{
	public class CategoryRepository : EfRepository<Category>, ICategoryRepository
	{
		public CategoryRepository(GardenShopDbContext db) : base(db) { }

		public Task<List<Category>> GetAllAsync(CancellationToken ct) =>
			Db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);

		public Task<Category?> GetByIdWithProductsAsync(long id, CancellationToken ct) =>
			Db.Categories.AsNoTracking()
				.Include(c => c.Products)
				.FirstOrDefaultAsync(c => c.Id == id, ct);

		public Task<bool> NameExistsAsync(string name, long? exceptId, CancellationToken ct)
		{
			var n = name.Trim().ToLowerInvariant();
			return Db.Categories.AnyAsync(c => (exceptId == null || c.Id != exceptId) && c.Name.ToLower() == n, ct);
		}

		public Task<bool> IsUsedByProductsAsync(long categoryId, CancellationToken ct) =>
			Db.Products.AnyAsync(p => p.CategoryId == categoryId, ct);
	}

}
