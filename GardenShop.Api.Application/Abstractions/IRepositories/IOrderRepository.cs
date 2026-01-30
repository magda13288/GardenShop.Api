using GardenShop.Domain.Entities;

namespace GardenShop.Application.Abstractions.Persistence
{
	public interface IOrderRepository : IRepository<Order>
	{
		Task<List<Order>> GetAllAsync(CancellationToken ct);
		Task<Order?> GetByIdWithItemsAsync(int id, CancellationToken ct);
		Task<Order?> GetByIdWithItemsAndProductsAsync(int id, CancellationToken ct);
	}
}
