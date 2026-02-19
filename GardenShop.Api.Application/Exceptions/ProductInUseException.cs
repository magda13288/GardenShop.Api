namespace GardenShop.Application.Exceptions
{
	public class ProductInUseException : ConflictException
	{
		public ProductInUseException(long productId) : base($"Product {productId} is used in orders.") { }
	}
}
