using static GardenShop.Application.DTO.CategoryDto;
using static GardenShop.Application.Requests.CategoryRequests;

namespace GardenShop.Application.Abstractions.IServices
{
	public interface ICategoryService
	{
		Task<List<CategoryListItemDto>> GetListAsync(CancellationToken ct);
		Task<CategoryDetailsDto> GetDetailsAsync(long categoryId, CancellationToken ct);

		Task<CategoryListItemDto> CreateAsync(CreateCategoryRequest req, CancellationToken ct);
		Task<CategoryListItemDto> UpdateAsync(long categoryId, UpdateCategoryRequest req, CancellationToken ct);
		Task DeleteAsync(long categoryId, CancellationToken ct);
	}
}
