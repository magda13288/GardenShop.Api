using GardenShop.Domain.Common;

namespace GardenShop.Application.Abstractions.Persistence
{
	public interface IRepository<T> where T : BaseEntity
	{
		Task<T?> GetByIdAsync(int id, CancellationToken ct);

		void Add(T entity);
		void Remove(T entity);
	}
}
