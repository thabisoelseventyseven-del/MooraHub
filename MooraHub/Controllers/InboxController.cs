using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MooraHub.Data;
using MooraHub.Models;

namespace MooraHub.Controllers
{
    [Authorize]
    public class InboxController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public InboxController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // /Inbox -> redirect based on role
        [HttpGet("/Inbox")]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(Admin));

            return RedirectToAction(nameof(My));
        }

        // ✅ FIX: If someone browses /Inbox/Send (GET), redirect to Checkout instead of 405
        [HttpGet("/Inbox/Send")]
        public IActionResult Send()
        {
            return RedirectToAction("Checkout", "Cart");
        }

        // ✅ POST /Inbox/Send (form submit)
        [HttpPost("/Inbox/Send")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(string UserMessage, string SelectedServices, int TotalAmount)
        {
            if (string.IsNullOrWhiteSpace(UserMessage))
                return RedirectToAction(nameof(My));

            var userId = _userManager.GetUserId(User) ?? "";
            var email = User.Identity?.Name ?? "";

            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction(nameof(My));

            var ticket = new SupportTicket
            {
                UserId = userId,
                UserEmail = email,
                SelectedServices = SelectedServices ?? "",
                TotalAmount = TotalAmount,
                UserMessage = UserMessage.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _db.SupportTickets.Add(ticket);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(My));
        }

        // USER: list own tickets
        [HttpGet("/Inbox/My")]
        public async Task<IActionResult> My()
        {
            var userId = _userManager.GetUserId(User) ?? "";

            var tickets = await _db.SupportTickets
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // ADMIN: view all tickets
        [HttpGet("/Inbox/Admin")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Admin()
        {
            var tickets = await _db.SupportTickets
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // ADMIN: reply
        [HttpPost("/Inbox/Reply")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Reply(int id, string adminReply)
        {
            var ticket = await _db.SupportTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ticket.AdminReply = adminReply?.Trim();
            ticket.IsReplied = true;
            ticket.RepliedAt = DateTime.UtcNow;

            // Badge logic (if you have it in model)
            ticket.UserUnreadReply = true;
            if (ticket.Status == "New")
                ticket.Status = "InProgress";

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Admin));
        }

        // ADMIN: set status
        [HttpPost("/Inbox/SetStatus")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SetStatus(int id, string status)
        {
            var ticket = await _db.SupportTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            var allowed = new[] { "New", "InProgress", "Completed" };
            ticket.Status = allowed.Contains(status) ? status : "New";

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Admin));
        }
    }
}
