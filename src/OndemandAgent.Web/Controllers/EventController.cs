using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OndemandAgent.Web.Data;
using OndemandAgent.Web.Models;
using OndemandAgent.Web.Dtos;
using System.Security.Claims;

namespace OndemandAgent.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var tenantIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(tenantIdString)) return Unauthorized();

            var tenantId = Guid.Parse(tenantIdString);

            return await _context.Events
                .Where(e => e.TenantId == tenantId)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent(CreateEventRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("Kimlik bilgisi doğrulanamadı.");
            }

            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.Parse(userIdString), 
                Name = request.Name,
                StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc),
                IsActive = request.IsActive,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventById), new { id = newEvent.Id }, newEvent);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEventById(Guid id)
        {
            var tenantIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(tenantIdString)) return Unauthorized();

            var tenantId = Guid.Parse(tenantIdString);

            var myEvent = await _context.Events
                .Include(e => e.Documents) 
                .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId);

            if (myEvent == null) return NotFound("Etkinlik bulunamadı.");

            return Ok(myEvent);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(Guid id, CreateEventRequest request)
        {
            var tenantIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(tenantIdString)) return Unauthorized();
            var tenantId = Guid.Parse(tenantIdString);

            var existingEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId);

            if (existingEvent == null) return NotFound("Etkinlik bulunamadı.");

            existingEvent.Name = request.Name;
            existingEvent.Description = request.Description;
            existingEvent.StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
            existingEvent.EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);
            
            existingEvent.IsActive = request.IsActive; 

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/logs")]
        public async Task<ActionResult<List<ChatLogDto>>> GetEventLogs(Guid id)
        {
            var tenantIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(tenantIdString)) return Unauthorized();
            var tenantId = Guid.Parse(tenantIdString);

            var isMyEvent = await _context.Events.AnyAsync(e => e.Id == id && e.TenantId == tenantId);
            if (!isMyEvent) return Unauthorized("Bu etkinliğin loglarını görme yetkiniz yok.");

            var logs = await _context.ChatLogs
                .Where(c => c.EventId == id)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ChatLogDto
                {
                    UserQuestion = c.UserQuestion,
                    AgentAnswer = c.AgentAnswer,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(logs);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var tenantIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(tenantIdString)) return Unauthorized();
            var tenantId = Guid.Parse(tenantIdString);

            var eventToDelete = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId);

            if (eventToDelete == null) return NotFound("Etkinlik bulunamadı veya silme yetkiniz yok.");

            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}