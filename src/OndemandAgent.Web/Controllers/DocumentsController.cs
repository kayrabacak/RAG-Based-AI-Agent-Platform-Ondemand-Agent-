using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OndemandAgent.Web.Data;
using OndemandAgent.Web.Dtos;
using OndemandAgent.Web.Models;
using OndemandAgent.Web.Services;
using System.IO;

namespace OndemandAgent.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IAIService _aiService;

        public DocumentsController(AppDbContext context, IWebHostEnvironment environment, IAIService aiService)
        {
            _context = context;
            _environment = environment;
            _aiService = aiService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadDocumentRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Lütfen geçerli bir dosya yükleyin.");

            var myEvent = await _context.Events.FindAsync(request.EventId);
            if (myEvent == null)
                return NotFound("Belirtilen etkinlik bulunamadı.");

            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(request.File.FileName)}_{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            var newDocument = new Document
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                FileName = request.File.FileName,
                FilePath = filePath,
                Status = ProcessingStatus.Pending,
                UploadedAt = DateTime.UtcNow
            };

            _context.Documents.Add(newDocument);
            await _context.SaveChangesAsync();

            bool aiSuccess = await _aiService.IndexDocumentAsync(request.EventId, newDocument.Id, filePath);

            if (aiSuccess)
            {
                newDocument.Status = ProcessingStatus.Completed;
            }
            else
            {
                newDocument.Status = ProcessingStatus.Failed;
            }

            await _context.SaveChangesAsync();

            return Ok(new { 
                message = aiSuccess ? "Dosya yüklendi ve AI tarafından öğrenildi." : "Dosya yüklendi ama AI işleyemedi.", 
                path = $"/uploads/{uniqueFileName}",
                aiStatus = newDocument.Status.ToString()
            });
        }
    }
}