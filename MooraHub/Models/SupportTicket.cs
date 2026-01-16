using System.ComponentModel.DataAnnotations;

namespace MooraHub.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }

        // ✅ Required by DB (NOT NULL)
        [Required]
        public string UserId { get; set; } = "";

        public string UserEmail { get; set; } = "";

        public string SelectedServices { get; set; } = "";
        public int TotalAmount { get; set; }

        [Required]
        public string UserMessage { get; set; } = "";

        // ✅ NEW: Payment method selected on checkout
        public string PaymentMethod { get; set; } = "EFT";

        // ✅ NEW: Saved file paths (relative to wwwroot)
        public string? ProofOfPaymentPath { get; set; }
        public string? VoiceNotePath { get; set; }

        // Admin reply
        public string? AdminReply { get; set; }
        public bool IsReplied { get; set; }
        public DateTime? RepliedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
