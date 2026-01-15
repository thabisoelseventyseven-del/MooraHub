using System;

namespace MooraHub.Models
{
    // EF Core entity must be a class with settable properties
    public class SupportTicket
    {
        public int Id { get; set; }

        // User identity
        public string UserId { get; set; } = "";
        public string UserEmail { get; set; } = "";

        // What user selected
        public string SelectedServices { get; set; } = "";
        public int TotalAmount { get; set; }

        // Conversation
        public string UserMessage { get; set; } = "";
        public string? AdminReply { get; set; }

        // Status
        public bool IsReplied { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RepliedAt { get; set; }
    }
}
