namespace GardenShop.Application.DTO
{
	public class CategoryDto
	{
		public record CategoryListItemDto(long Id, string Name);
		public record CategoryDetailsDto(long Id, string Name, List<CategoryProductListItemDto> Products);

		public record CategoryProductListItemDto(
			long Id,
			string Name,
			string? Description,
			decimal Price,
			int Stock);
	}
}
