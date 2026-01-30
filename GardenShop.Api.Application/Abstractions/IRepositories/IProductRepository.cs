using GardenShop.Domain.Entities;

namespace GardenShop.Application.Abstractions.Persistence
{
	public interface IProductRepository : IRepository<Product>
	{
		Task<List<Product>> GetAllAsync(int? categoryId, CancellationToken ct);
		Task<List<Product>> GetAllWithCategoryAsync(int? categoryId, CancellationToken ct);

		Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken ct);

		Task<bool> CategoryExistsAsync(int categoryId, CancellationToken ct);
		Task<bool> IsUsedByOrdersAsync(int productId, CancellationToken ct);

		Task<List<Product>> GetByIdsTrackingAsync(IEnumerable<int> ids, CancellationToken ct);
	}
}
