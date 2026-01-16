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

    // Add item and stay on Dashboard (so user can add more)
    [HttpGet]
    public IActionResult Add(int id, string? returnTo = null)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge(new Microsoft.AspNetCore.Authentication.AuthenticationProperties
            {
                RedirectUri = Url.Action("Add", "Cart", new { id, returnTo })
            });
        }

        _cart.Add(HttpContext, id);

        // default: go back to Dashboard to add more
        if (!string.IsNullOrWhiteSpace(returnTo))
            return Redirect(returnTo);

        return RedirectToAction("Dashboard", "Home");
    }

    // Add item and go straight to checkout
    [Authorize]
    [HttpGet]
    public IActionResult BuyNow(int id)
    {
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
