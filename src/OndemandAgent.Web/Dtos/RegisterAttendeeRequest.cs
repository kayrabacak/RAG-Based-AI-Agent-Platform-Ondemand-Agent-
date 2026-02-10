using System.ComponentModel.DataAnnotations;

namespace OndemandAgent.Web.Dtos
{
    public class RegisterAttendeeRequest
    {
        public Guid EventId { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [EmailAddress]
        public string? Email { get; set; }
    }
}
