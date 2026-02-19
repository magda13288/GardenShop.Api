using GardenShop.Domain.Enums;

namespace GardenShop.Application.DTO
{
	public record OrderListItemDto(
	long Id,
	DateTimeOffset CreatedAtUtc,
	string CustomerName,
	OrderStatus Status,
	decimal Total);

	public record FullOrder(
		long Id,
		DateTimeOffset CreatedAtUtc,
		string CustomerName,
		OrderStatus Status,
		decimal Total,
		List<FullOrderItem> Items);

	public record FullOrderItem(
		long ProductId,
		string ProductName,
		decimal UnitPrice,
		int Quantity);
}
