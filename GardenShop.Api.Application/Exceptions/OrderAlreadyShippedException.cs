namespace GardenShop.Application.Exceptions
{
	public class OrderAlreadyShippedException : ConflictException
	{
		public OrderAlreadyShippedException(long orderId) : base($"Order {orderId} is shipped and cannot be cancelled.") { }
	}
}
