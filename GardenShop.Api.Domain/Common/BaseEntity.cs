using System.ComponentModel.DataAnnotations.Schema;

namespace GardenShop.Domain.Common
{
	public class BaseEntity
	{
		protected BaseEntity(int id)
		{
			this.Id = id;
		}

		protected BaseEntity()
		{
			this.Id = 0;
		}
		public int Id { get; set; }
		public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
		public DateTimeOffset? UpdatedAtUtc { get; set; }
	}
}

