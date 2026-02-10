using System.ComponentModel.DataAnnotations;

namespace OndemandAgent.Web.Models
{
    public class Attendee
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; } 

        [Required]
        public required string Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; } 

        public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
    }
}