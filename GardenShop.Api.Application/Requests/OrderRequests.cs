namespace GardenShop.Application.Requests
{
	public class OrderRequests
	{
		public record CreateOrderItemRequest(long ProductId, int Quantity);
		public record CreateOrderRequest(string CustomerName, List<CreateOrderItemRequest> Items);

	}
}
