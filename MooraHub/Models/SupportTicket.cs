using System.ComponentModel.DataAnnotations;

namespace MooraHub.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }

        // ✅ Required (NOT NULL in DB)
        [Required]
        public string UserId { get; set; } = "";

        public string UserEmail { get; set; } = "";

        public string SelectedServices { get; set; } = "";

        public int TotalAmount { get; set; }

        [Required]
        public string UserMessage { get; set; } = "";

        // ✅ Payment info
        public string PaymentMethod { get; set; } = "CashSend (Capitec) - 0721769099";

        // ✅ Uploads (paths saved in DB)
        public string? ProofOfPaymentPath { get; set; }
        public string? VoiceNotePath { get; set; }

        // ✅ Admin reply
        public string? AdminReply { get; set; }
        public bool IsReplied { get; set; }
        public DateTime? RepliedAt { get; set; }

        // ✅ Admin voice note (optional)
        public string? AdminVoiceNotePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
