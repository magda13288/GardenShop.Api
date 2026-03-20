using GardenShop.Application.Abstractions.IServices;
using GardenShop.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using static GardenShop.Application.DTO.CategoryDto;
using static GardenShop.Application.Requests.CategoryRequests;

namespace GardenShop.Api.Controllers
{
	[ApiController]
	[Route("api/categories")]
	public sealed class CategoriesController : ControllerBase
	{
		private readonly ICategoryService _categoryService;
		private readonly ILogger<CategoriesController> logger;

		public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
		{
			_categoryService = categoryService;
			this.logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<List<CategoryListItemDto>>> GetCategoriesAsync(CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Getting categories.");
				var list = await _categoryService.GetListAsync(ct);
				return this.Ok(list);
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while getting categories.");
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet("{categoryId:long}")]
		public async Task<ActionResult<CategoryDetailsDto>> GetCategoryAsync(long categoryId, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Getting category {CategoryId}.", categoryId);
				var dto = await _categoryService.GetDetailsAsync(categoryId, ct);
				return this.Ok(dto);
			}
			catch (CategoryNotFoundException ex)
			{
				this.logger.LogError(ex, "Category {CategoryId} not found.", categoryId);
				return this.NotFound();
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while getting category {CategoryId}.", categoryId);
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpPost]
		public async Task<ActionResult<CategoryListItemDto>> CreateCategoryAsync([FromBody] CreateCategoryRequest req, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Creating category '{Name}'.", req.Name);
				var dto = await _categoryService.CreateAsync(req, ct);
				return this.Created($"/api/categories/{dto.Id}", dto);
			}
			catch (ValidationException ex)
			{
				this.logger.LogError(ex, "Validation error while creating category.");
				return this.BadRequest(ex.Message);
			}
			catch (CategoryNameConflictException ex)
			{
				this.logger.LogError(ex, "Category name conflict while creating category.");
				return this.Conflict();
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while creating category.");
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpPut("{categoryId:long}")]
		public async Task<ActionResult<CategoryListItemDto>> UpdateCategoryAsync(long categoryId, [FromBody] UpdateCategoryRequest req, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Updating category {CategoryId}.", categoryId);
				var dto = await _categoryService.UpdateAsync(categoryId, req, ct);
				return this.Ok(dto);
			}
			catch (CategoryNotFoundException ex)
			{
				this.logger.LogError(ex, "Category {CategoryId} not found.", categoryId);
				return this.NotFound();
			}
			catch (ValidationException ex)
			{
				this.logger.LogError(ex, "Validation error while updating category {CategoryId}.", categoryId);
				return this.BadRequest(ex.Message);
			}
			catch (CategoryNameConflictException ex)
			{
				this.logger.LogError(ex, "Category name conflict while updating category {CategoryId}.", categoryId);
				return this.Conflict();
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while updating category {CategoryId}.", categoryId);
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpDelete("{categoryId:long}")]
		public async Task<IActionResult> DeleteCategoryAsync(long categoryId, CancellationToken ct)
		{
			try
			{
				this.logger.LogTrace("Deleting category {CategoryId}.", categoryId);
				await _categoryService.DeleteAsync(categoryId, ct);
				return this.NoContent();
			}
			catch (CategoryNotFoundException ex)
			{
				this.logger.LogError(ex, "Category {CategoryId} not found.", categoryId);
				return this.NotFound();
			}
			catch (CategoryInUseException ex)
			{
				this.logger.LogError(ex, "Category {CategoryId} is in use.", categoryId);
				return this.Conflict();
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Unhandled exception while deleting category {CategoryId}.", categoryId);
				return this.StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}

}
