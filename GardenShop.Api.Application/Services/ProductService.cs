using GardenShop.Application.Abstractions.IServices;
using GardenShop.Application.Abstractions.Persistence;
using GardenShop.Application.Exceptions;
using GardenShop.Application.Mappings;
using GardenShop.Domain.Entities;
using Microsoft.Extensions.Logging;
using static GardenShop.Application.DTO.ProductDto;
using static GardenShop.Application.Requests.ProductRequests;

namespace GardenShop.Application.Services
{
	public class ProductService: IProductService
	{
		private readonly IProductRepository _repo;
		private readonly IUnitOfWork _uow;
		private readonly ILogger<ProductService> _logger;

		public ProductService(IProductRepository repo, IUnitOfWork uow, ILogger<ProductService> logger)
		{
			_repo = repo;
			_uow = uow;
			_logger = logger;
		}

		public async Task<List<ProductListItemDto>> GetListAsync(long? categoryId, CancellationToken ct)
		{
			var list = await _repo.GetAllAsync(categoryId, ct);
			return list.Select(Mappers.MapToProductListItem).ToList();
		}

		public async Task<List<ProductListItemWithCategoryDto>> GetListWithCategoryAsync(long? categoryId, CancellationToken ct)
		{
			var list = await _repo.GetAllWithCategoryAsync(categoryId, ct);
			return list.Select(Mappers.MapToProductListItemWithCategory).ToList();
		}

		public async Task<ProductDetailsDto> GetDetailsAsync(long productId, CancellationToken ct)
		{
			var entity = await _repo.GetByIdAsync(productId, ct);
			if (entity is null) throw new ProductNotFoundException(productId);
			return Mappers.MapToProductDetails(entity);
		}

		public async Task<ProductDetailsWithCategoryDto> GetDetailsWithCategoryAsync(long productId, CancellationToken ct)
		{
			var entity = await _repo.GetByIdWithCategoryAsync(productId, ct);
			if (entity is null) throw new ProductNotFoundException(productId);
			return Mappers.MapToProductDetailsWithCategory(entity);
		}

		public async Task<ProductDetailsDto> CreateAsync(CreateProductRequest req, CancellationToken ct)
		{
			Validate(req.Name, req.Price, req.Stock);

			if (!await _repo.CategoryExistsAsync(req.CategoryId, ct))
				throw new ValidationException("CategoryId does not exist.");

			var entity = new Product
			{
				Name = req.Name.Trim(),
				Description = req.Description?.Trim(),
				Price = req.Price,
				Stock = req.Stock,
				CategoryId = req.CategoryId
			};

			_repo.Add(entity);
			await _uow.SaveChangesAsync(ct);

			_logger.LogInformation("Product created: {ProductId} {Name}", entity.Id, entity.Name);
			return Mappers.MapToProductDetails(entity);
		}

		public async Task<ProductDetailsDto> UpdateAsync(long productId, UpdateProductRequest req, CancellationToken ct)
		{
			var entity = await _repo.GetByIdAsync(productId, ct);
			if (entity is null) throw new ProductNotFoundException(productId);

			Validate(req.Name, req.Price, req.Stock);

			if (!await _repo.CategoryExistsAsync(req.CategoryId, ct))
				throw new ValidationException("CategoryId does not exist.");

			entity.Name = req.Name.Trim();
			entity.Description = req.Description?.Trim();
			entity.Price = req.Price;
			entity.Stock = req.Stock;
			entity.CategoryId = req.CategoryId;
			entity.UpdatedAtUtc = DateTimeOffset.UtcNow;

			await _uow.SaveChangesAsync(ct);

			_logger.LogInformation("Product updated: {ProductId}", entity.Id);
			return Mappers.MapToProductDetails(entity);
		}

		public async Task DeleteAsync(long productId, CancellationToken ct)
		{
			var entity = await _repo.GetByIdAsync(productId, ct);
			if (entity is null) throw new ProductNotFoundException(productId);

			if (await _repo.IsUsedByOrdersAsync(productId, ct))
				throw new ProductInUseException(productId);

			_repo.Remove(entity);
			await _uow.SaveChangesAsync(ct);

			_logger.LogInformation("Product deleted: {ProductId}", productId);
		}

		private static void Validate(string name, decimal price, int stock)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Name is required.");
			if (price < 0) throw new ValidationException("Price must be >= 0.");
			if (stock < 0) throw new ValidationException("Stock must be >= 0.");
		}
	}
}
