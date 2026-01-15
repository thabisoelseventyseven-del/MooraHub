using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MooraHub.Data;
using MooraHub.Models;
using System.Security.Claims;

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

        // ✅ /Inbox
        public IActionResult Index()
        {
            return RedirectToAction("My");
        }

        // ✅ User inbox
        public async Task<IActionResult> My()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tickets = await _db.SupportTickets
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // ✅ Admin inbox
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Admin()
        {
            var tickets = await _db.SupportTickets
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // ✅ Send from Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(string userMessage, string selectedServices, int totalAmount)
        {
            var ticket = new SupportTicket
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
                UserEmail = User.Identity?.Name ?? "",
                SelectedServices = selectedServices ?? "",
                TotalAmount = totalAmount,
                UserMessage = userMessage ?? "",
                CreatedAt = DateTime.UtcNow
            };

            _db.SupportTickets.Add(ticket);
            await _db.SaveChangesAsync();

            return RedirectToAction("My");
        }

        // ✅ Admin reply
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Reply(int id, string adminReply)
        {
            var ticket = await _db.SupportTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ticket.AdminReply = adminReply;
            ticket.IsReplied = true;
            ticket.RepliedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return RedirectToAction("Admin");
        }
    }
}
