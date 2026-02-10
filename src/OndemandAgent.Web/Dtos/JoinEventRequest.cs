using System.ComponentModel.DataAnnotations;

namespace OndemandAgent.Web.Dtos
{
    public class JoinEventRequest
    {
        [Required]
        public required string Name { get; set; } // "Mehmet Yılmaz"

        [EmailAddress]
        public string? Email { get; set; } // Opsiyonel
    }
}