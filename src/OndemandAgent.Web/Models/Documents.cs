using System.ComponentModel.DataAnnotations;

namespace OndemandAgent.Web.Models
{
    public enum ProcessingStatus
    {
        Pending,    
        Processing, 
        Completed,  
        Failed      
    }

    public class Document
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; } 
        
        [Required]
        public required string FileName { get; set; }

        [Required]
        public required string FilePath { get; set; } 

        public ProcessingStatus Status { get; set; } = ProcessingStatus.Pending;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}