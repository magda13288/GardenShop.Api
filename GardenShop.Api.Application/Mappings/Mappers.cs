using GardenShop.Application.DTO;
using GardenShop.Domain.Entities;
using static GardenShop.Application.DTO.CategoryDto;
using static GardenShop.Application.DTO.ProductDto;

namespace GardenShop.Application.Mappings
{
	public class Mappers
	{
		public static CategoryListItemDto Map(Category c) => new(c.Id, c.Name);

		public static CategoryDetailsDto MapToCategoryDetails(Category c) =>
			new(
				c.Id,
				c.Name,
				c.Products
					.OrderBy(p => p.Name)
					.Select(p => new CategoryProductListItemDto(p.Id, p.Name, p.Description, p.Price, p.Stock))
					.ToList());

		public static ProductListItemDto MapToProductListItem(Product p) =>
			new(p.Id, p.Name, p.Price, p.Stock, p.CategoryId);

		public static ProductListItemWithCategoryDto MapToProductListItemWithCategory(Product p) =>
			new(p.Id, p.Name, p.Price, p.Stock, p.CategoryId, p.Category.Name);

		public static ProductDetailsDto MapToProductDetails(Product p) =>
			new(p.Id, p.Name, p.Description, p.Price, p.Stock, p.CategoryId);

		public static ProductDetailsWithCategoryDto MapToProductDetailsWithCategory(Product p) =>
			new(p.Id, p.Name, p.Description, p.Price, p.Stock, p.CategoryId, p.Category.Name);

		public static OrderListItemDto MapToOrderListItem(Order o) =>
			new(o.Id, o.CreatedAtUtc, o.CustomerName, o.Status, o.Total);

		public static FullOrder MapToFullOrder(Order o) =>
			new(
				o.Id,
				o.CreatedAtUtc,
				o.CustomerName,
				o.Status,
				o.Total,
				o.Items.Select(i => new FullOrderItem(i.ProductId, i.ProductNameSnapshot, i.UnitPrice, i.Quantity)).ToList());
	}
}

