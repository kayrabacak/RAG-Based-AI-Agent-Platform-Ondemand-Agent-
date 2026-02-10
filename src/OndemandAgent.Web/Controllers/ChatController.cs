using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OndemandAgent.Web.Data;
using OndemandAgent.Web.Dtos;
using OndemandAgent.Web.Models;
using OndemandAgent.Web.Services;

namespace OndemandAgent.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAIService _aiService;

        public ChatController(AppDbContext context, IAIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public class AskQuestionRequest
        {
            public Guid EventId { get; set; }
            public Guid? AttendeeId { get; set; }
            public required string Question { get; set; }
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskQuestionRequest request)
        {
            var targetEvent = await _context.Events.FindAsync(request.EventId);
            
            var now = DateTime.UtcNow;
            var isWithinDateRange = targetEvent != null && now >= targetEvent.StartDate && now <= targetEvent.EndDate;

            if (targetEvent == null || !targetEvent.IsActive || !isWithinDateRange)
                return BadRequest("Etkinlik aktif değil, süresi dolmuş veya henüz başlamamış.");

            string answer = await _aiService.ChatAsync(request.EventId, request.Question);

            var chatLog = new ChatLog
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                AttendeeId = request.AttendeeId,
                UserQuestion = request.Question,
                AgentAnswer = answer,
                DetectedTopic = "General",
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatLogs.Add(chatLog);
            await _context.SaveChangesAsync();

            return Ok(new { answer = answer });
        }

        [HttpGet("info/{eventId}")]
        public async Task<IActionResult> GetEventInfo(Guid eventId)
        {
            var targetEvent = await _context.Events
                .Where(e => e.Id == eventId)
                .FirstOrDefaultAsync();

            if (targetEvent == null)
                return NotFound("Etkinlik bulunamadı.");

            var now = DateTime.UtcNow;
            var isWithinDateRange = now >= targetEvent.StartDate && now <= targetEvent.EndDate;

            var effectiveIsActive = targetEvent.IsActive && isWithinDateRange;

            var eventInfo = new 
            { 
                targetEvent.Id, 
                targetEvent.Name, 
                IsActive = effectiveIsActive,
                targetEvent.Description 
            };

            return Ok(eventInfo);
        }
    }
}