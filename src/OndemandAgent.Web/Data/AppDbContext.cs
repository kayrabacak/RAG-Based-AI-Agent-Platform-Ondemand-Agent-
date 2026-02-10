using Microsoft.EntityFrameworkCore;
using OndemandAgent.Web.Models;

namespace OndemandAgent.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

       public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<ChatLog> ChatLogs { get; set; }
    }
}