namespace GardenShop.Application.Requests
{
	public class ProductRequests
	{
		public record CreateProductRequest(string Name, string? Description, decimal Price, int Stock, long CategoryId);
		public record UpdateProductRequest(string Name, string? Description, decimal Price, int Stock, long CategoryId);
	}
}
