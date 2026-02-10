namespace OndemandAgent.Web.Models
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        

        public string CompanyName { get; set; } = string.Empty;

        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}