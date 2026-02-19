namespace GardenShop.Application.Requests
{
	public class CategoryRequests
	{
		public record CreateCategoryRequest(string Name);
		public record UpdateCategoryRequest(string Name);
	}
}
