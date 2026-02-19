namespace GardenShop.Application.Exceptions
{
	public class ProductNotFoundException : AppException
	{
		public long ProductId { get; }
		public ProductNotFoundException(long productId) : base($"Product with id {productId} not found.") { ProductId = productId; }
	}
}
