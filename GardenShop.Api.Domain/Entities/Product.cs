using GardenShop.Domain.Common;

namespace GardenShop.Domain.Entities
{
	public class Product:BaseEntity
	{
		public required string Name { get; set; }
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public int Stock { get; set; }

		public int CategoryId { get; set; }
		public Category Category { get; set; } = null!;

		public List<OrderItem> OrderItems { get; set; } = [];
	}
}
