using GardenShop.Domain.Entities;

namespace GardenShop.Application.Abstractions.Persistence
{
	public interface IOrderRepository : IRepository<Order>
	{
		Task<List<Order>> GetAllAsync(CancellationToken ct);
		Task<Order?> GetByIdWithItemsAsync(long id, CancellationToken ct);
		Task<Order?> GetByIdWithItemsAndProductsAsync(long id, CancellationToken ct);
	}
}
