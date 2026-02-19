using GardenShop.Domain.Entities;

namespace GardenShop.Application.Abstractions.Persistence
{
	public interface ICategoryRepository : IRepository<Category>
	{
		Task<List<Category>> GetAllAsync(CancellationToken ct);
		Task<Category?> GetByIdWithProductsAsync(long id, CancellationToken ct);

		Task<bool> NameExistsAsync(string name, long? exceptId, CancellationToken ct);
		Task<bool> IsUsedByProductsAsync(long categoryId, CancellationToken ct);
	}
}
