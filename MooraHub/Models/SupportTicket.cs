using System.ComponentModel.DataAnnotations;

namespace MooraHub.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }

        // Required
        [Required]
        public string UserId { get; set; } = "";

        public string UserEmail { get; set; } = "";

        public string SelectedServices { get; set; } = "";
        public int TotalAmount { get; set; }

        [Required]
        public string UserMessage { get; set; } = "";

        // Reply
        public string? AdminReply { get; set; }
        public bool IsReplied { get; set; }
        public DateTime? RepliedAt { get; set; }

        // Uploads / payments (already in your project)
        public string PaymentMethod { get; set; } = "CashSend (Capitec) - 0721769099";
        public string? ProofOfPaymentPath { get; set; }
        public string? VoiceNotePath { get; set; }
        public string? AdminVoiceNotePath { get; set; }

        // ✅ Upgrade #1: Ticket workflow status
        // New | InProgress | Completed
        [Required]
        public string Status { get; set; } = "New";

        // ✅ Badge logic
        // When admin replies -> UserUnreadReply = true
        // When user opens ticket -> UserUnreadReply = false
        public bool UserUnreadReply { get; set; } = false;

        // Created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
