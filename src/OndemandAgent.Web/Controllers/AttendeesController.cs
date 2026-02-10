using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OndemandAgent.Web.Data;
using OndemandAgent.Web.Dtos;
using OndemandAgent.Web.Models;

namespace OndemandAgent.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttendeesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterAttendeeRequest request)
        {
            var ev = await _context.Events.FindAsync(request.EventId);
            if (ev == null) return NotFound("Etkinlik bulunamadı.");

            if (!string.IsNullOrEmpty(request.Email))
            {
                var existing = await _context.Attendees
                    .FirstOrDefaultAsync(a => a.EventId == request.EventId && a.Email == request.Email);
                
                if (existing != null)
                {
                    return Ok(new { existing.Id, existing.Name, Message = "Tekrar hoşgeldiniz." });
                }
            }

            var newAttendee = new Attendee
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                Name = request.Name,
                Email = request.Email,
                VisitedAt = DateTime.UtcNow
            };

            _context.Attendees.Add(newAttendee);
            await _context.SaveChangesAsync();

            return Ok(new { newAttendee.Id, newAttendee.Name });
        }
    }
}