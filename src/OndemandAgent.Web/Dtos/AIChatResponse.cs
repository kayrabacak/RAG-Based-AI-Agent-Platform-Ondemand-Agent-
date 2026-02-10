using System.Text.Json.Serialization;

namespace OndemandAgent.Web.Dtos
{
    public class AIChatResponse
    {
        [JsonPropertyName("answer")]
        public string Answer { get; set; } = string.Empty;
    }
}