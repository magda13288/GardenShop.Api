using static GardenShop.Application.DTO.ProductDto;
using static GardenShop.Application.Requests.ProductRequests;

namespace GardenShop.Application.Abstractions.IServices
{
	public interface IProductService
	{
		Task<List<ProductListItemDto>> GetListAsync(long? categoryId, CancellationToken ct);
		Task<List<ProductListItemWithCategoryDto>> GetListWithCategoryAsync(long? categoryId, CancellationToken ct);

		Task<ProductDetailsDto> GetDetailsAsync(long productId, CancellationToken ct);
		Task<ProductDetailsWithCategoryDto> GetDetailsWithCategoryAsync(long productId, CancellationToken ct);

		Task<ProductDetailsDto> CreateAsync(CreateProductRequest req, CancellationToken ct);
		Task<ProductDetailsDto> UpdateAsync(long productId, UpdateProductRequest req, CancellationToken ct);
		Task DeleteAsync(long productId, CancellationToken ct);
	}
}
