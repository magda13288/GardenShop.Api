using Microsoft.AspNetCore.Mvc;

namespace GardenShop.Api.Controllers
{
	public class ProductsController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
