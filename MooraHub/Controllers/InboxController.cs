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

        // Your admin email (only this user can access Admin inbox)
        private const string AdminEmail = "thabisoelseventyseven@gmail.com";

        public InboxController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ✅ POST from Checkout -> creates a ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(string SelectedServices, int TotalAmount, string UserMessage)
        {
            if (string.IsNullOrWhiteSpace(UserMessage))
                return RedirectToAction("Checkout", "Cart");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var ticket = new SupportTicket
            {
                UserId = user.Id,
                UserEmail = user.Email ?? "",
                SelectedServices = SelectedServices ?? "",
                TotalAmount = TotalAmount,
                UserMessage = UserMessage.Trim()
            };

            _db.SupportTickets.Add(ticket);
            await _db.SaveChangesAsync();

            // ✅ after sending -> user sees their inbox
            return RedirectToAction(nameof(My));
        }

        // ✅ USER INBOX: only tickets created by this user
        [HttpGet]
        public async Task<IActionResult> My()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var tickets = await _db.SupportTickets
                .Where(t => t.UserId == user.Id)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // ✅ ADMIN INBOX: only admin email can open this page
        [HttpGet]
        public async Task<IActionResult> Admin()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.Email?.ToLower() != AdminEmail.ToLower())
                return Forbid();

            var tickets = await _db.SupportTickets
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // ✅ ADMIN REPLY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, string reply)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.Email?.ToLower() != AdminEmail.ToLower())
                return Forbid();

            var ticket = await _db.SupportTickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null) return NotFound();

            ticket.AdminReply = (reply ?? "").Trim();
            ticket.RepliedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Admin));
        }
    }
}
