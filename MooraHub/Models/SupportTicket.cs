using System.ComponentModel.DataAnnotations;

namespace MooraHub.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }

        // ✅ Required by your DB (NOT NULL)
        [Required]
        public string UserId { get; set; } = "";

        // Helpful for display
        public string UserEmail { get; set; } = "";

        public string SelectedServices { get; set; } = "";
        public int TotalAmount { get; set; }

        [Required]
        public string UserMessage { get; set; } = "";

        // Admin reply
        public string? AdminReply { get; set; }
        public bool IsReplied { get; set; }
        public DateTime? RepliedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
