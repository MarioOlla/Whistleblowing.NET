using Microsoft.AspNetCore.Mvc;

namespace Whistleblowing.NET.Controllers
{
	public class SegnalazioniAnonimeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
