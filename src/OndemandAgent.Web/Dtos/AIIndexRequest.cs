using System.Text.Json.Serialization;

namespace OndemandAgent.Web.Dtos
{
    public class AIIndexRequest
    {
        // Python tarafındaki 'event_id' ismine uyması için JsonPropertyName kullanıyoruz
        [JsonPropertyName("event_id")]
        public string EventId { get; set; } = string.Empty;

        [JsonPropertyName("document_id")]
        public string DocumentId { get; set; } = string.Empty;

        [JsonPropertyName("file_path")]
        public string FilePath { get; set; } = string.Empty;
    }
}