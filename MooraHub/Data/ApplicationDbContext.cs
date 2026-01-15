using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MooraHub.Models;

namespace MooraHub.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ✅ Inbox tickets table
        public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    }
}
