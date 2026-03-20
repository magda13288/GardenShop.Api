using GardenShop.Application.Abstractions.IServices;
using GardenShop.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using static GardenShop.Application.DTO.ProductDto;
using static GardenShop.Application.Requests.ProductRequests;

namespace GardenShop.Api.Controllers
{
	[ApiController]
	[Route("api/products")]
	public sealed class ProductsController : ControllerBase
	{
		private readonly IProductService _productService;
		private readonly ILogger<ProductsController> logger;

		public ProductsController(IProductService productService, ILogger<ProductsController> logger)
		{
			_productService = productService;
			this.logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<List<ProductListItemDto>>> GetProductsAsync([FromQuery] long? categoryId, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Getting products (categoryId={CategoryId}).", categoryId);
				var list = await _productService.GetListAsync(categoryId, ct);
				return this.Ok(list);
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while getting products.");
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet("with-category")]
		public async Task<ActionResult<List<ProductListItemWithCategoryDto>>> GetProductsWithCategoryAsync([FromQuery] long? categoryId, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Getting products with category (categoryId={CategoryId}).", categoryId);
				var list = await _productService.GetListWithCategoryAsync(categoryId, ct);
				return this.Ok(list);
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while getting products with category.");
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet("{productId:long}")]
		public async Task<ActionResult<ProductDetailsDto>> GetProductAsync(long productId, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Getting product {ProductId}.", productId);
				var dto = await _productService.GetDetailsAsync(productId, ct);
				return this.Ok(dto);
			}
			catch (ProductNotFoundException ex)
			{
				this.logger.LogError(ex, "Product {ProductId} not found.", productId);
				return this.NotFound();
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while getting product {ProductId}.", productId);
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet("{productId:long}/with-category")]
		public async Task<ActionResult<ProductDetailsWithCategoryDto>> GetProductWithCategoryAsync(long productId, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Getting product {ProductId} with category.", productId);
				var dto = await _productService.GetDetailsWithCategoryAsync(productId, ct);
				return this.Ok(dto);
			}
			catch (ProductNotFoundException ex)
			{
				this.logger.LogError(ex, "Product {ProductId} not found.", productId);
				return this.NotFound();
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while getting product {ProductId} with category.", productId);
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpPost]
		public async Task<ActionResult<ProductDetailsDto>> CreateProductAsync([FromBody] CreateProductRequest req, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Creating product '{Name}'.", req.Name);
				var dto = await _productService.CreateAsync(req, ct);
				return this.Created($"/api/products/{dto.Id}", dto);
			}
			catch (ValidationException ex)
			{
				this.logger.LogError(ex, "Validation error while creating product.");
				return this.BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while creating product.");
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpPut("{productId:long}")]
		public async Task<ActionResult<ProductDetailsDto>> UpdateProductAsync(long productId, [FromBody] UpdateProductRequest req, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Updating product {ProductId}.", productId);
				var dto = await _productService.UpdateAsync(productId, req, ct);
				return this.Ok(dto);
			}
			catch (ProductNotFoundException ex)
			{
				this.logger.LogError(ex, "Product {ProductId} not found.", productId);
				return this.NotFound();
			}
			catch (ValidationException ex)
			{
				this.logger.LogError(ex, "Validation error while updating product {ProductId}.", productId);
				return this.BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while updating product {ProductId}.", productId);
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpDelete("{productId:long}")]
		public async Task<IActionResult> DeleteProductAsync(long productId, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Deleting product {ProductId}.", productId);
				await _productService.DeleteAsync(productId, ct);
				return this.NoContent();
			}
			catch (ProductNotFoundException ex)
			{
				this.logger.LogError(ex, "Product {ProductId} not found.", productId);
				return this.NotFound();
			}
			catch (ProductInUseException ex)
			{
				this.logger.LogError(ex, "Product {ProductId} is in use.", productId);
				return this.Conflict();
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while deleting product {ProductId}.", productId);
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}

}
