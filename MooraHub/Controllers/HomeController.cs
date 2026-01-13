using Microsoft.AspNetCore.Mvc;

namespace MooraHub.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // NEW: Dashboard Page
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
