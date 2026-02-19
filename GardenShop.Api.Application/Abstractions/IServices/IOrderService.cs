using GardenShop.Application.DTO;
using static GardenShop.Application.Requests.OrderRequests;

namespace GardenShop.Application.Abstractions.IServices
{
	public interface IOrderService
	{
		Task<List<OrderListItemDto>> GetListAsync(CancellationToken ct);
		Task<FullOrder> GetOrderAsync(long orderId, CancellationToken ct);

		Task<long> CreateAsync(CreateOrderRequest req, CancellationToken ct);
		Task CancelAsync(long orderId, CancellationToken ct);
	}
}
