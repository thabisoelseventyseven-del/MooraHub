using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MooraHub.Data;

namespace MooraHub.Services
{
    public class InboxBadgeService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public InboxBadgeService(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<int> GetUnreadCountAsync(System.Security.Claims.ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            if (string.IsNullOrWhiteSpace(userId)) return 0;

            return await _db.SupportTickets
                .Where(t => t.UserId == userId && t.UserUnreadReply)
                .CountAsync();
        }
    }
}
