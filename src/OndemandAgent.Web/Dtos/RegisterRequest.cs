using System.ComponentModel.DataAnnotations;

namespace OndemandAgent.Web.Dtos
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        public string CompanyName { get; set; } = string.Empty;
    }
}