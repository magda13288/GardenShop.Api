using GardenShop.Application.Abstractions.Persistence;
using GardenShop.Domain.Entities;
using GardenShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GardenShop.Infrastructure.Repositories
{
	public class OrderRepository : EfRepository<Order>, IOrderRepository
	{
		public OrderRepository(GardenShopDbContext db) : base(db) { }

		public Task<List<Order>> GetAllAsync(CancellationToken ct) =>
			Db.Orders.AsNoTracking()
				.OrderByDescending(o => o.CreatedAtUtc)
				.ToListAsync(ct);

		public Task<Order?> GetByIdWithItemsAsync(long id, CancellationToken ct) =>
			Db.Orders.AsNoTracking()
				.Include(o => o.Items)
				.FirstOrDefaultAsync(o => o.Id == id, ct);

		public Task<Order?> GetByIdWithItemsAndProductsAsync(long id, CancellationToken ct) =>
			Db.Orders
				.Include(o => o.Items)
					.ThenInclude(i => i.Product)
				.FirstOrDefaultAsync(o => o.Id == id, ct);
	}
}
