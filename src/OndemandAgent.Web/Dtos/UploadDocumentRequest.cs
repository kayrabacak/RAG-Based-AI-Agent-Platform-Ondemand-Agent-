using Microsoft.AspNetCore.Http; // IFormFile için gerekli
using System;

namespace OndemandAgent.Web.Dtos
{
    public class UploadDocumentRequest
    {
        // Dosyanın hangi etkinliğe ait olduğunu bilmeliyiz
        public Guid EventId { get; set; }

        // C#'ta dosya transferi için 'IFormFile' kullanılır.
        // Bu, dosyanın içeriğini, adını ve boyutunu taşıyan bir pakettir.
        public required IFormFile File { get; set; } 
    }
}