using System.Text.Json.Serialization;

namespace OndemandAgent.Web.Dtos
{
    public class AIChatRequest
    {
        [JsonPropertyName("event_id")]
        public string EventId { get; set; } = string.Empty;

        [JsonPropertyName("question")]
        public string Question { get; set; } = string.Empty;
    }
}