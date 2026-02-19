namespace GardenShop.Domain.Entities
{
	public class OrderItem
	{
		public long OrderId { get; set; }
		public Order Order { get; set; } = null!;

		public long ProductId { get; set; }
		public Product Product { get; set; } = null!;

		public required string ProductNameSnapshot { get; set; }
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
	}
}
