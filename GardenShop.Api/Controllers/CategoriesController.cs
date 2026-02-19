using Microsoft.AspNetCore.Mvc;

namespace GardenShop.Api.Controllers
{
	public class CategoriesController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
