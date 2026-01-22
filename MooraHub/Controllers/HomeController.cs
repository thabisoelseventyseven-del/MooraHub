using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MooraHub.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Admin", "Inbox");

            // If user is already logged in, send them to Dashboard
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction(nameof(Dashboard));

            // If not logged in, show Home page
            return View();
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult CareerPathfinder()
        {
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        // NOTE: Checkout is normally in CartController. Keep only if you truly have a View here.
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }
    }
}
