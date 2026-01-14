using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MooraHub.Services;

namespace MooraHub.Controllers;

public class CartController : Controller
{
    private readonly CartSessionService _cart;

    public CartController(CartSessionService cart)
    {
        _cart = cart;
    }

    // ✅ Click service -> if not logged in, go login and return here
    [HttpGet]
    public IActionResult Add(int id)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge(new Microsoft.AspNetCore.Authentication.AuthenticationProperties
            {
                RedirectUri = Url.Action("Add", "Cart", new { id })
            });
        }

        _cart.Add(HttpContext, id);
        return RedirectToAction("Checkout");
    }

    [Authorize]
    [HttpGet]
    public IActionResult Checkout()
    {
        var cart = _cart.GetCart(HttpContext);
        ViewBag.Total = _cart.Total(HttpContext);
        return View(cart);
    }

    [Authorize]
    [HttpPost]
    public IActionResult Remove(int index)
    {
        _cart.RemoveAt(HttpContext, index);
        return RedirectToAction("Checkout");
    }

    [Authorize]
    [HttpPost]
    public IActionResult Clear()
    {
        _cart.Clear(HttpContext);
        return RedirectToAction("Checkout");
    }
}
