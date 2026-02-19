namespace GardenShop.Application.Exceptions
{
	public class OrderAlreadyCancelledException : ConflictException
	{
		public OrderAlreadyCancelledException(long orderId) : base($"Order {orderId} is already cancelled.") { }
	}
}
