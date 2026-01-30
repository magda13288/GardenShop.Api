using GardenShop.Domain.Common;
using GardenShop.Domain.Enums;

namespace GardenShop.Domain.Entities
{
	public class Order:BaseEntity
	{
		public required string CustomerName { get; set; }
		public OrderStatus Status { get; set; } = OrderStatus.New;
		public decimal Total { get; set; }

		public List<OrderItem> Items { get; set; } = new();
	}
}
