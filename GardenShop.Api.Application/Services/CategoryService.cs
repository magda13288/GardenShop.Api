using GardenShop.Application.Abstractions.IServices;
using GardenShop.Application.Abstractions.Persistence;
using GardenShop.Application.Exceptions;
using GardenShop.Application.Mappings;
using GardenShop.Domain.Entities;
using static GardenShop.Application.DTO.CategoryDto;
using static GardenShop.Application.Requests.CategoryRequests;

namespace GardenShop.Application.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly ICategoryRepository _repo;
		private readonly IUnitOfWork _uow;
		private readonly ILogger<CategoryService> _logger;

		public CategoryService(ICategoryRepository repo, IUnitOfWork uow, ILogger<CategoryService> logger)
		{
			_repo = repo;
			_uow = uow;
			_logger = logger;
		}

		public async Task<List<CategoryListItemDto>> GetListAsync(CancellationToken ct)
		{
			var list = await _repo.GetAllAsync(ct);
			return list.Select(Mappers.Map).ToList();
		}

		public async Task<CategoryDetailsDto> GetDetailsAsync(long categoryId, CancellationToken ct)
		{
			var entity = await _repo.GetByIdWithProductsAsync(categoryId, ct);
			if (entity is null) throw new CategoryNotFoundException(categoryId);

			return Mappers.MapToCategoryDetails(entity);
		}

		public async Task<CategoryListItemDto> CreateAsync(CreateCategoryRequest req, CancellationToken ct)
		{
			if (string.IsNullOrWhiteSpace(req.Name)) throw new ValidationException("Name is required.");

			if (await _repo.NameExistsAsync(req.Name, exceptId: null, ct))
				throw new CategoryNameConflictException(req.Name);

			var entity = new Category { Name = req.Name.Trim() };
			_repo.Add(entity);
			await _uow.SaveChangesAsync(ct);

			_logger.LogInformation("Category created: {CategoryId} {Name}", entity.Id, entity.Name);
			return Mappers.Map(entity);
		}

		public async Task<CategoryListItemDto> UpdateAsync(long categoryId, UpdateCategoryRequest req, CancellationToken ct)
		{
			var entity = await _repo.GetByIdAsync(categoryId, ct);
			if (entity is null) throw new CategoryNotFoundException(categoryId);

			if (string.IsNullOrWhiteSpace(req.Name)) throw new ValidationException("Name is required.");

			if (await _repo.NameExistsAsync(req.Name, exceptId: categoryId, ct))
				throw new CategoryNameConflictException(req.Name);

			entity.Name = req.Name.Trim();
			entity.UpdatedAtUtc = DateTimeOffset.UtcNow;

			await _uow.SaveChangesAsync(ct);
			_logger.LogInformation("Category updated: {CategoryId}", entity.Id);

			return Mappers.Map(entity);
		}

		public async Task DeleteAsync(long categoryId, CancellationToken ct)
		{
			var entity = await _repo.GetByIdAsync(categoryId, ct);
			if (entity is null) throw new CategoryNotFoundException(categoryId);

			if (await _repo.IsUsedByProductsAsync(categoryId, ct))
				throw new CategoryInUseException(categoryId);

			_repo.Remove(entity);
			await _uow.SaveChangesAsync(ct);

			_logger.LogInformation("Category deleted: {CategoryId}", categoryId);
		}
	}
}
