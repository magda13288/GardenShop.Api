using System.ComponentModel.DataAnnotations.Schema;

namespace GardenShop.Domain.Common
{
	public class BaseEntity
	{
		protected BaseEntity(long id)
		{
			this.Id = id;
		}

		protected BaseEntity()
		{
			this.Id = 0;
		}
		public long Id { get; set; }
		public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
		public DateTimeOffset? UpdatedAtUtc { get; set; }
	}
}

