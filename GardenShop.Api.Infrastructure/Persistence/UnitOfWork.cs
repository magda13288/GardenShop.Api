using GardenShop.Application.Abstractions.Persistence;

namespace GardenShop.Infrastructure.Persistence
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly GardenShopDbContext _context;
		public UnitOfWork(GardenShopDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

		public Task<int> SaveChangesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
	}
}
