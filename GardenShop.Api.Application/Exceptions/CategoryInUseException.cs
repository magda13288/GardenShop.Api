namespace GardenShop.Application.Exceptions
{
	public class CategoryInUseException : ConflictException
	{
		public CategoryInUseException(long categoryId) : base($"Category {categoryId} is used by products.") { }
	}
}
