using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OndemandAgent.Web.Models
{
    public class ChatLog
    {
        public Guid Id { get; set; }

   
        public Guid EventId { get; set; }
        

        public Guid? AttendeeId { get; set; } 


        [ForeignKey("EventId")]
        public virtual Event Event { get; set; } 

        public string UserQuestion { get; set; } = string.Empty;

        public string AgentAnswer { get; set; } = string.Empty;

        public string DetectedTopic { get; set; } = string.Empty; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}