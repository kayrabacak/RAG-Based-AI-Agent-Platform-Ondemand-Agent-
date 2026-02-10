using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OndemandAgent.Web.Data;
using OndemandAgent.Web.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OndemandAgent.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize] 
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<AnalyticsSummaryDto>> GetSummary()
        {
            
            var tenantIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(tenantIdString)) return Unauthorized();
            var tenantId = Guid.Parse(tenantIdString);

            
            var myLogsQuery = _context.ChatLogs
                .Include(c => c.Event) 
                .Where(c => c.Event.TenantId == tenantId);

            var totalQuestions = await myLogsQuery.CountAsync();

            var topQuestions = await myLogsQuery
                .GroupBy(c => c.UserQuestion)
                .Select(g => new TopQuestionDto
                {
                    Text = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            var eventStats = await myLogsQuery
                .GroupBy(c => new { c.EventId, c.Event.Name })
                .Select(g => new EventStatDto
                {
                    EventId = g.Key.EventId,
                    EventName = g.Key.Name,
                    QuestionCount = g.Count()
                })
                .OrderByDescending(x => x.QuestionCount)
                .ToListAsync();
            
            return Ok(new AnalyticsSummaryDto
            {
                TotalQuestions = totalQuestions,
                TopQuestions = topQuestions,
                EventStats = eventStats
            });
        }

        [HttpGet("events/{eventId}")]
        public async Task<ActionResult<AnalyticsSummaryDto>> GetEventDetails(Guid eventId)
        {
            var tenantIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(tenantIdString)) return Unauthorized();
            var tenantId = Guid.Parse(tenantIdString);

            var targetEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == eventId && e.TenantId == tenantId);

            if (targetEvent == null)
            {
                return NotFound("Etkinlik bulunamadı veya erişim yetkiniz yok.");
            }

            var query = _context.ChatLogs.Where(c => c.EventId == eventId);

            var totalQuestions = await query.CountAsync();

            var topQuestions = await query
                .GroupBy(c => c.UserQuestion)
                .Select(g => new TopQuestionDto
                {
                    Text = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();
            
            var eventStats = new System.Collections.Generic.List<EventStatDto>
            {
                new EventStatDto
                {
                    EventId = eventId,
                    EventName = targetEvent.Name,
                    QuestionCount = totalQuestions
                }
            };

            return Ok(new AnalyticsSummaryDto
            {
                TotalQuestions = totalQuestions,
                TopQuestions = topQuestions,
                EventStats = eventStats
            });
        }
    }
}
