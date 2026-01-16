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
        private readonly IWebHostEnvironment _env;

        public InboxController(
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _env = env;
        }

        // ✅ /Inbox works
        [HttpGet]
        public IActionResult Index()
        {
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

        // ✅ USER: send message from checkout (with uploads)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(
            string UserMessage,
            string SelectedServices,
            int TotalAmount,
            string PaymentMethod,
            IFormFile? ProofOfPayment,
            IFormFile? VoiceNote)
        {
            if (string.IsNullOrWhiteSpace(UserMessage))
                return RedirectToAction(nameof(My));

            var userId = _userManager.GetUserId(User);
            var email = User.Identity?.Name ?? "";

            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction(nameof(My));

            // ✅ Ensure upload folders exist
            var paymentsDir = Path.Combine(_env.WebRootPath, "uploads", "payments");
            var voicesDir = Path.Combine(_env.WebRootPath, "uploads", "voices");
            Directory.CreateDirectory(paymentsDir);
            Directory.CreateDirectory(voicesDir);

            // ✅ Save ProofOfPayment (optional)
            string? proofPath = null;
            if (ProofOfPayment != null && ProofOfPayment.Length > 0)
            {
                var ext = Path.GetExtension(ProofOfPayment.FileName);
                var fileName = $"pay_{Guid.NewGuid():N}{ext}";
                var fullPath = Path.Combine(paymentsDir, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await ProofOfPayment.CopyToAsync(stream);

                // store relative path so you can link it later
                proofPath = $"/uploads/payments/{fileName}";
            }

            // ✅ Save VoiceNote (optional)
            string? voicePath = null;
            if (VoiceNote != null && VoiceNote.Length > 0)
            {
                var ext = Path.GetExtension(VoiceNote.FileName);
                var fileName = $"voice_{Guid.NewGuid():N}{ext}";
                var fullPath = Path.Combine(voicesDir, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await VoiceNote.CopyToAsync(stream);

                voicePath = $"/uploads/voices/{fileName}";
            }

            var ticket = new SupportTicket
            {
                UserId = userId,
                UserEmail = email,
                SelectedServices = SelectedServices ?? "",
                TotalAmount = TotalAmount,
                UserMessage = UserMessage.Trim(),
                PaymentMethod = string.IsNullOrWhiteSpace(PaymentMethod) ? "EFT" : PaymentMethod,
                ProofOfPaymentPath = proofPath,
                VoiceNotePath = voicePath,
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

        // ✅ ADMIN: reply
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
