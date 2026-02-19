namespace GardenShop.Application.Exceptions
{
	public class InsufficientStockException : ConflictException
	{
		public long ProductId { get; }
		public int Available { get; }
		public int Requested { get; }

		public InsufficientStockException(long productId, int available, int requested)
			: base($"Insufficient stock for product {productId}. Available={available}, requested={requested}.")
		{
			ProductId = productId;
			Available = available;
			Requested = requested;
		}
	}
}
