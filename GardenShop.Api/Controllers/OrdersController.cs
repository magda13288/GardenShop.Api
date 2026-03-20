using GardenShop.Application.Abstractions.IServices;
using GardenShop.Application.DTO;
using GardenShop.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using static GardenShop.Application.Requests.OrderRequests;

namespace GardenShop.Api.Controllers
{
	[ApiController]
	[Route("api/orders")]
	public sealed class OrdersController : ControllerBase
	{
		private readonly IOrderService _orderService;
		private readonly ILogger<OrdersController> logger;

		public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
		{
			_orderService = orderService;
			this.logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<List<OrderListItemDto>>> GetOrdersAsync(CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Getting orders.");
				var list = await _orderService.GetListAsync(ct);
				return this.Ok(list);
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while getting orders.");
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet("{orderId:long}")]
		public async Task<ActionResult<FullOrder>> GetOrderAsync(long orderId, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Getting order {OrderId}.", orderId);

				var order = await this._orderService.GetOrderAsync(orderId, ct);
				return this.Ok(order);
			}
			catch (OrderNotFoundException ex)
			{
				this.logger.LogError(ex, "Order {OrderId} not found.", orderId);
				return this.NotFound();
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while getting order {OrderId}.", orderId);
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpPost]
		public async Task<ActionResult<object>> CreateOrderAsync([FromBody] CreateOrderRequest req, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Creating order for '{CustomerName}'.", req.CustomerName);

				var id = await _orderService.CreateAsync(req, ct);
				return this.Created($"/api/orders/{id}", new { id });
			}
			catch (ValidationException ex)
			{
				this.logger.LogError(ex, "Validation error while creating order.");
				return this.BadRequest(ex.Message);
			}
			catch (InsufficientStockException ex)
			{
				this.logger.LogError(ex, "Insufficient stock while creating order. Product={ProductId}", ex.ProductId);
				return this.Conflict();
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while creating order.");
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpPost("{orderId:long}/cancel")]
		public async Task<IActionResult> CancelOrderAsync(long orderId, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Cancelling order {OrderId}.", orderId);

				await _orderService.CancelAsync(orderId, ct);
				return this.Ok();
			}
			catch (OrderNotFoundException ex)
			{
				this.logger.LogError(ex, "Order {OrderId} not found.", orderId);
				return this.NotFound();
			}
			catch (ConflictException ex)
			{
				this.logger.LogError(ex, "Conflict while cancelling order {OrderId}.", orderId);
				return this.Conflict();
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while cancelling order {OrderId}.", orderId);
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
