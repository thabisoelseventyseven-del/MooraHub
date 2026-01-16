using Microsoft.AspNetCore.Authorization;
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

        public InboxController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ✅ USER: list own tickets
        [HttpGet]
        public async Task<IActionResult> My()
        {
            var email = User.Identity?.Name ?? "";
            var tickets = await _db.SupportTickets
                .Where(t => t.UserEmail == email)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // ✅ USER: send message from checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(string userMessage, string selectedServices, int totalAmount)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return RedirectToAction("My");

            var email = User.Identity?.Name ?? "";

            var ticket = new SupportTicket
            {
                UserEmail = email,
                SelectedServices = selectedServices ?? "",
                TotalAmount = totalAmount,
                UserMessage = userMessage.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _db.SupportTickets.Add(ticket);
            await _db.SaveChangesAsync();

            return RedirectToAction("My");
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

            return RedirectToAction("Admin");
        }
    }
}
