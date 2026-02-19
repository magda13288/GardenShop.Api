namespace GardenShop.Application.Exceptions
{
	public class CategoryNameConflictException : ConflictException
	{
		public CategoryNameConflictException(string name) : base($"Category name '{name}' already exists.") { }
	}
}
