using GardenShop.Application.Abstractions.Persistence;
using GardenShop.Domain.Entities;
using GardenShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GardenShop.Infrastructure.Repositories
{
	public class ProductRepository : EfRepository<Product>, IProductRepository
	{
		public ProductRepository(GardenShopDbContext db) : base(db) { }

		public Task<List<Product>> GetAllAsync(long? categoryId, CancellationToken ct)
		{
			var q = Db.Products.AsNoTracking().AsQueryable();
			if (categoryId is not null) q = q.Where(p => p.CategoryId == categoryId);
			return q.OrderBy(p => p.Name).ToListAsync(ct);
		}

		public Task<List<Product>> GetAllWithCategoryAsync(long? categoryId, CancellationToken ct)
		{
			var q = Db.Products.AsNoTracking().Include(p => p.Category).AsQueryable();
			if (categoryId is not null) q = q.Where(p => p.CategoryId == categoryId);
			return q.OrderBy(p => p.Name).ToListAsync(ct);
		}

		public Task<Product?> GetByIdWithCategoryAsync(long id, CancellationToken ct) =>
			Db.Products.AsNoTracking()
				.Include(p => p.Category)
				.FirstOrDefaultAsync(p => p.Id == id, ct);

		public Task<bool> CategoryExistsAsync(long categoryId, CancellationToken ct) =>
			Db.Categories.AnyAsync(c => c.Id == categoryId, ct);

		public Task<bool> IsUsedByOrdersAsync(long productId, CancellationToken ct) =>
			Db.OrderItems.AnyAsync(oi => oi.ProductId == productId, ct);

		public Task<List<Product>> GetByIdsTrackingAsync(IEnumerable<long> ids, CancellationToken ct)
		{
			var list = ids.Distinct().ToList();
			return Db.Products.Where(p => list.Contains(p.Id)).ToListAsync(ct);
		}
	}
}
