using System;
using System.ComponentModel.DataAnnotations;

namespace OndemandAgent.Web.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        

        public Guid? TenantId { get; set; } 
        public Tenant? Tenant { get; set; }

        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();
        public ICollection<ChatLog> ChatLogs { get; set; } = new List<ChatLog>();
    }
}