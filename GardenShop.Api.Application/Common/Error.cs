namespace GardenShop.Application.Common
{
	public sealed record Error(string Title, string? Detail = null);
}
