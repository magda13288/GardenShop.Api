namespace GardenShop.Application.Exceptions
{
	public class OrderNotFoundException: AppException
	{
		public long OrderId { get; }
		public OrderNotFoundException(long orderId) : base($"Order with id {orderId} not found.") { OrderId = orderId; }
	}
}
