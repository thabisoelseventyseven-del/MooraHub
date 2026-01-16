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

        // ✅ FIX: /Inbox now works
        [HttpGet]
        public IActionResult Index()
        {
            // If admin, go to Admin inbox, else go to My inbox
            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(Admin));

            return RedirectToAction(nameof(My));
        }

        // ✅ USER: list own tickets
        [HttpGet]
        public async Task<IActionResult> My()
        {
            var userId = _userManager.GetUserId(User) ?? "";

            var tickets = await _db.SupportTickets
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // ✅ USER: send message from checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(string UserMessage, string SelectedServices, int TotalAmount)
        {
            if (string.IsNullOrWhiteSpace(UserMessage))
                return RedirectToAction(nameof(My));

            var userId = _userManager.GetUserId(User);
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

        // ✅ ADMIN: view all tickets
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Admin()
        {
            var tickets = await _db.SupportTickets
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // ✅ ADMIN: reply to a ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Reply(int id, string adminReply)
        {
            var ticket = await _db.SupportTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ticket.AdminReply = adminReply?.Trim();
            ticket.IsReplied = true;
            ticket.RepliedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Admin));
        }
    }
}
