using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MooraHub.Controllers
{
    [Authorize] // 🔒 Only logged-in users can access
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
