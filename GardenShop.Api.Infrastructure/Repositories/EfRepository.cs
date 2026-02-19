using GardenShop.Application.Abstractions.Persistence;
using GardenShop.Domain.Common;
using GardenShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GardenShop.Infrastructure.Repositories
{
	public class EfRepository<T> : IRepository<T> where T : BaseEntity
	{
		protected readonly GardenShopDbContext Db;
		protected readonly DbSet<T> Set;

		public EfRepository(GardenShopDbContext db)
		{
			Db = db;
			Set = db.Set<T>();
		}

		public virtual Task<T?> GetByIdAsync(long id, CancellationToken ct) =>
			Set.FindAsync([id], ct).AsTask();

		public virtual void Add(T entity) => Set.Add(entity);
		public virtual void Remove(T entity) => Set.Remove(entity);
	}
}
