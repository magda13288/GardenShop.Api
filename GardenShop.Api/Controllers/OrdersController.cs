using Microsoft.AspNetCore.Mvc;

namespace GardenShop.Api.Controllers
{
	public class OrdersController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
