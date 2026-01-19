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
        [HttpGet]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(Admin));

            return RedirectToAction(nameof(My));
        }

        // USER: list own tickets
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

        // USER: open a ticket (marks reply as read)
        [HttpGet]
        public async Task<IActionResult> Ticket(int id)
        {
            var userId = _userManager.GetUserId(User) ?? "";

            var ticket = await _db.SupportTickets.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (ticket == null) return NotFound();

            if (ticket.UserUnreadReply)
            {
                ticket.UserUnreadReply = false;
                await _db.SaveChangesAsync();
            }

            return View(ticket);
        }

        // USER: delete one ticket (clear chat)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User) ?? "";

            var ticket = await _db.SupportTickets.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (ticket == null) return RedirectToAction(nameof(My));

            _db.SupportTickets.Remove(ticket);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(My));
        }

        // USER: clear all tickets
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearAll()
        {
            var userId = _userManager.GetUserId(User) ?? "";

            var myTickets = await _db.SupportTickets.Where(t => t.UserId == userId).ToListAsync();
            _db.SupportTickets.RemoveRange(myTickets);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(My));
        }

        // ADMIN: view all tickets
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Admin()
        {
            var tickets = await _db.SupportTickets
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // ADMIN: update status quickly (New/InProgress/Completed)
        [HttpPost]
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

        // ADMIN: reply to a ticket (marks unread for user)
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

            // ✅ user badge
            ticket.UserUnreadReply = true;

            // If admin replied, default status becomes InProgress (feel free)
            if (ticket.Status == "New")
                ticket.Status = "InProgress";

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Admin));
        }
    }
}
