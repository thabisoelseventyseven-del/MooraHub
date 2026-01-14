using System;
using System.ComponentModel.DataAnnotations;

namespace MooraHub.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        [Required]
        public string UserEmail { get; set; } = "";

        [Required]
        public string SelectedServices { get; set; } = "";

        public int TotalAmount { get; set; }

        [Required]
        [MaxLength(4000)]
        public string UserMessage { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(4000)]
        public string? AdminReply { get; set; }

        public DateTime? RepliedAt { get; set; }

        // ✅ This is what your views are calling
        public bool IsReplied => !string.IsNullOrWhiteSpace(AdminReply);
    }
}
