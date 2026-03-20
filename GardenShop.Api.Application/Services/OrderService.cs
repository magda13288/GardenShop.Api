using GardenShop.Application.Abstractions.IServices;
using GardenShop.Application.Abstractions.Persistence;
using GardenShop.Application.DTO;
using GardenShop.Application.Exceptions;
using GardenShop.Application.Mappings;
using GardenShop.Domain.Entities;
using GardenShop.Domain.Enums;
using Microsoft.Extensions.Logging;
using static GardenShop.Application.Requests.OrderRequests;

namespace GardenShop.Application.Services
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepo;
		private readonly IProductRepository _productRepo;
		private readonly IUnitOfWork _uow;
		private readonly ILogger<OrderService> _logger;

		public OrderService(
			IOrderRepository orderRepo,
			IProductRepository productRepo,
			IUnitOfWork uow,
			ILogger<OrderService> logger)
		{
			_orderRepo = orderRepo;
			_productRepo = productRepo;
			_uow = uow;
			_logger = logger;
		}

		public async Task<List<OrderListItemDto>> GetListAsync(CancellationToken ct)
		{
			var list = await _orderRepo.GetAllAsync(ct);
			return list.Select(Mappers.MapToOrderListItem).ToList();
		}

		public async Task<FullOrder> GetOrderAsync(long orderId, CancellationToken ct)
		{
			var order = await _orderRepo.GetByIdWithItemsAsync(orderId, ct);
			if (order is null) throw new OrderNotFoundException(orderId);
			return Mappers.MapToFullOrder(order);
		}

		public async Task<long> CreateAsync(CreateOrderRequest req, CancellationToken ct)
		{
			if (string.IsNullOrWhiteSpace(req.CustomerName))
				throw new ValidationException("CustomerName is required.");

			if (req.Items is null || req.Items.Count == 0)
				throw new ValidationException("Order must contain at least 1 item.");

			var itemsByProduct = req.Items
				.GroupBy(i => i.ProductId)
				.Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
				.ToList();

			if (itemsByProduct.Any(x => x.Quantity <= 0))
				throw new ValidationException("Item quantity must be > 0.");

			var productIds = itemsByProduct.Select(x => x.ProductId).Distinct().ToList();
			var products = await _productRepo.GetByIdsTrackingAsync(productIds, ct);

			if (products.Count != productIds.Count)
				throw new ValidationException("One or more products do not exist.");

			foreach (var r in itemsByProduct)
			{
				var p = products.Single(x => x.Id == r.ProductId);
				if (p.Stock < r.Quantity)
					throw new InsufficientStockException(p.Id, p.Stock, r.Quantity);
			}

			var order = new Order
			{
				CustomerName = req.CustomerName.Trim(),
				CreatedAtUtc = DateTimeOffset.UtcNow,
				Status = OrderStatus.New
			};

			foreach (var r in itemsByProduct)
			{
				var p = products.Single(x => x.Id == r.ProductId);
				p.Stock -= r.Quantity;

				order.Items.Add(new OrderItem
				{
					ProductId = p.Id,
					Product = p,
					ProductNameSnapshot = p.Name,
					Quantity = r.Quantity,
					UnitPrice = p.Price
				});
			}

			order.Total = order.Items.Sum(i => i.UnitPrice * i.Quantity);

			_orderRepo.Add(order);
			await _uow.SaveChangesAsync(ct);

			_logger.LogInformation("Order created: {OrderId} total={Total}", order.Id, order.Total);
			return order.Id;
		}

		public async Task CancelAsync(long orderId, CancellationToken ct)
		{
			var order = await _orderRepo.GetByIdWithItemsAndProductsAsync(orderId, ct);
			if (order is null) throw new OrderNotFoundException(orderId);

			if (order.Status == OrderStatus.Cancelled) throw new OrderAlreadyCancelledException(orderId);
			if (order.Status == OrderStatus.Shipped) throw new OrderAlreadyShippedException(orderId);

			foreach (var item in order.Items)
				item.Product.Stock += item.Quantity;

			order.Status = OrderStatus.Cancelled;
			order.UpdatedAtUtc = DateTimeOffset.UtcNow;

			await _uow.SaveChangesAsync(ct);
			_logger.LogInformation("Order cancelled: {OrderId}", orderId);
		}
	}
}
