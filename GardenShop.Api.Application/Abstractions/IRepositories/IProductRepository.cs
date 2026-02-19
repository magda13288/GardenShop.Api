using GardenShop.Domain.Entities;

namespace GardenShop.Application.Abstractions.Persistence
{
	public interface IProductRepository : IRepository<Product>
	{
		Task<List<Product>> GetAllAsync(long? categoryId, CancellationToken ct);
		Task<List<Product>> GetAllWithCategoryAsync(long? categoryId, CancellationToken ct);

		Task<Product?> GetByIdWithCategoryAsync(long id, CancellationToken ct);

		Task<bool> CategoryExistsAsync(long categoryId, CancellationToken ct);
		Task<bool> IsUsedByOrdersAsync(long productId, CancellationToken ct);

		Task<List<Product>> GetByIdsTrackingAsync(IEnumerable<long> ids, CancellationToken ct);
	}
}
