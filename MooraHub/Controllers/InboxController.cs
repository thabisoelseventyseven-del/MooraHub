using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MooraHub.Data;
using MooraHub.Models;

namespace MooraHub.Controllers;

[Authorize]
public class InboxController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    // Simple admin rule: YOUR email is admin
    private const string AdminEmail = "Thabisoelseventyseven@gmail.com";

    public InboxController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // USER INBOX
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Index", "Home");

        var tickets = await _db.SupportTickets
            .Where(t => t.UserId == user.Id)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return View(tickets);
    }

    // SEND FROM CHECKOUT
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(string SelectedServices, int TotalAmount, string UserMessage)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Index", "Home");

        if (string.IsNullOrWhiteSpace(UserMessage))
        {
            TempData["Err"] = "Please type a message in Sepedi or English before submitting.";
            return RedirectToAction("Checkout", "Cart");
        }

        var ticket = new SupportTicket
        {
            UserId = user.Id,
            UserEmail = user.Email ?? "",
            SelectedServices = SelectedServices ?? "",
            TotalAmount = TotalAmount,
            UserMessage = UserMessage.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.SupportTickets.Add(ticket);
        await _db.SaveChangesAsync();

        TempData["Ok"] = "Request sent. Check your Inbox for admin reply.";
        return RedirectToAction("Index");
    }

    // ADMIN INBOX
    public async Task<IActionResult> Admin()
    {
        if (!IsAdmin()) return Forbid();

        var tickets = await _db.SupportTickets
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return View(tickets);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(int id, string reply)
    {
        if (!IsAdmin()) return Forbid();

        var ticket = await _db.SupportTickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null) return NotFound();

        ticket.AdminReply = reply?.Trim();
        ticket.RepliedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return RedirectToAction("Admin");
    }

    private bool IsAdmin()
    {
        return (User.Identity?.IsAuthenticated == true) &&
               string.Equals(User.Identity?.Name, AdminEmail, StringComparison.OrdinalIgnoreCase);
    }
}
