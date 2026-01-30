using GardenShop.Domain.Entities;

namespace GardenShop.Application.Abstractions.Persistence
{
	public interface ICategoryRepository : IRepository<Category>
	{
		Task<List<Category>> GetAllAsync(CancellationToken ct);
		Task<Category?> GetByIdWithProductsAsync(int id, CancellationToken ct);

		Task<bool> NameExistsAsync(string name, int? exceptId, CancellationToken ct);
		Task<bool> IsUsedByProductsAsync(int categoryId, CancellationToken ct);
	}
}
