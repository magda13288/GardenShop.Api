namespace GardenShop.Application.Exceptions
{
	public class CategoryNotFoundException:AppException
	{
		public long CategoryId { get; }
		public CategoryNotFoundException(long categoryId) : base($"Category with id {categoryId} not found.") { CategoryId = categoryId; }
	}
}
