using System.ComponentModel.DataAnnotations;

namespace MooraHub.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }

        [Required]
        public string UserEmail { get; set; } = "";

        [Required]
        public string SelectedServices { get; set; } = "";

        public int TotalAmount { get; set; }

        [Required]
        public string UserMessage { get; set; } = "";

        // Admin reply
        public string? AdminReply { get; set; }

        public bool IsReplied { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RepliedAt { get; set; }
    }
}
