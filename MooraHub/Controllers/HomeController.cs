using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult About()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
            return RedirectToAction("Admin", "Inbox");

        // If user is already logged in, send them to Dashboard
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Dashboard");
        }
        // If not logged in, show Home page
        return View();
    }

    [Authorize]
    public IActionResult Dashboard()
    {
        return View();
    }
    [Authorize]
    public IActionResult Checkout()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
