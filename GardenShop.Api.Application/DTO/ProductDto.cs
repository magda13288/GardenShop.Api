namespace GardenShop.Application.DTO
{
	public class ProductDto
	{
		public record ProductListItemDto(
	long Id,
	string Name,
	decimal Price,
	int Stock,
	long CategoryId);

		public record ProductListItemWithCategoryDto(
			long Id,
			string Name,
			decimal Price,
			int Stock,
			long CategoryId,
			string CategoryName);

		public record ProductDetailsDto(
			long Id,
			string Name,
			string? Description,
			decimal Price,
			int Stock,
			long CategoryId);

		public record ProductDetailsWithCategoryDto(
			long Id,
			string Name,
			string? Description,
			decimal Price,
			int Stock,
			long CategoryId,
			string CategoryName);
	}
}
