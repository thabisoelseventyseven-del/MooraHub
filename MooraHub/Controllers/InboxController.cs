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

        public InboxController(ApplicationDbContext db, UserManager<IdentityUser> userManager, IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _env = env;
        }

        // /Inbox -> redirect based on role
        [HttpGet]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(Admin));

            return RedirectToAction(nameof(My));
        }

        // USER inbox
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

        // USER send from checkout
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

            // Ensure upload folder exists
            var uploadDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadDir);

            string? proofPath = null;
            if (ProofOfPayment != null && ProofOfPayment.Length > 0)
            {
                var proofName = $"{Guid.NewGuid():N}_{Path.GetFileName(ProofOfPayment.FileName)}";
                var proofFull = Path.Combine(uploadDir, proofName);
                using var fs = new FileStream(proofFull, FileMode.Create);
                await ProofOfPayment.CopyToAsync(fs);
                proofPath = "/uploads/" + proofName;
            }

            string? voicePath = null;
            if (VoiceNote != null && VoiceNote.Length > 0)
            {
                var voiceName = $"{Guid.NewGuid():N}_{Path.GetFileName(VoiceNote.FileName)}";
                var voiceFull = Path.Combine(uploadDir, voiceName);
                using var fs = new FileStream(voiceFull, FileMode.Create);
                await VoiceNote.CopyToAsync(fs);
                voicePath = "/uploads/" + voiceName;
            }

            var ticket = new SupportTicket
            {
                UserId = userId,
                UserEmail = email,
                SelectedServices = SelectedServices ?? "",
                TotalAmount = TotalAmount,
                UserMessage = UserMessage.Trim(),
                PaymentMethod = string.IsNullOrWhiteSpace(PaymentMethod)
                    ? "CashSend (Capitec) - 0721769099"
                    : PaymentMethod,
                ProofOfPaymentPath = proofPath,
                VoiceNotePath = voicePath,
                CreatedAt = DateTime.UtcNow
            };

            _db.SupportTickets.Add(ticket);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(My));
        }

        // ADMIN inbox
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Admin()
        {
            var tickets = await _db.SupportTickets
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // ✅ ADMIN reply (NOW supports voice note upload)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Reply(int id, string adminReply, IFormFile? AdminVoiceNote)
        {
            var ticket = await _db.SupportTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ticket.AdminReply = adminReply?.Trim();
            ticket.IsReplied = true;
            ticket.RepliedAt = DateTime.UtcNow;

            // ✅ Save admin voice note (optional)
            if (AdminVoiceNote != null && AdminVoiceNote.Length > 0)
            {
                var adminVoiceDir = Path.Combine(_env.WebRootPath, "uploads", "admin-voice");
                Directory.CreateDirectory(adminVoiceDir);

                var fileName = $"{Guid.NewGuid():N}_{Path.GetFileName(AdminVoiceNote.FileName)}";
                var fullPath = Path.Combine(adminVoiceDir, fileName);

                using var fs = new FileStream(fullPath, FileMode.Create);
                await AdminVoiceNote.CopyToAsync(fs);

                ticket.AdminVoiceNotePath = "/uploads/admin-voice/" + fileName;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Admin));
        }
    }
}
