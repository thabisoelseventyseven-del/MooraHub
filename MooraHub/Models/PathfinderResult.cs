using System;
using System.ComponentModel.DataAnnotations;

namespace MooraHub.Models
{
    public class PathfinderResult
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        public int? APS { get; set; }

        [MaxLength(20)]
        public string MathType { get; set; } = "";

        [MaxLength(30)]
        public string Interest { get; set; } = "";

        [MaxLength(30)]
        public string Style { get; set; } = "";

        [MaxLength(30)]
        public string Province { get; set; } = "";

        [MaxLength(10)]
        public string Risk { get; set; } = "mid";

        // Store recommendations as JSON (simple and flexible)
        [Required]
        public string RecommendationsJson { get; set; } = "[]";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
