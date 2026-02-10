using System.Net.Http.Json;

namespace OndemandAgent.UI.Services
{
    public class ChatService
    {
        private readonly HttpClient _httpClient;

        public ChatService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            var baseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(baseUrl ?? "http://localhost:5000");
        }

        public async Task<string> AskQuestionAsync(string eventId, string question, string? attendeeId = null)
        {
            try
            {
                var request = new { EventId = eventId, Question = question, AttendeeId = attendeeId };
                
                var response = await _httpClient.PostAsJsonAsync("/api/chat/ask", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ChatResponse>();
                    return result?.Answer ?? "Cevap boş döndü.";
                }
                else
                {
                    return "Üzgünüm, şu an sunucuya ulaşamıyorum.";
                }
            }
            catch (Exception ex)
            {
                return $"Bağlantı Hatası: {ex.Message}";
            }
        }

        public class ChatResponse
        {
            public string Answer { get; set; } = string.Empty;
        }

        public async Task<AttendeeDto?> RegisterAttendeeAsync(Guid eventId, string name, string? email)
        {
            try
            {
                var request = new { EventId = eventId, Name = name, Email = email };
                var response = await _httpClient.PostAsJsonAsync("/api/attendees", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AttendeeDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public class AttendeeDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        public async Task<EventInfoDto?> GetEventInfoAsync(string eventId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<EventInfoDto>($"/api/chat/info/{eventId}");
            }
            catch
            {
                return null;
            }
        }

        public class EventInfoDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public string Description { get; set; } = string.Empty;
        }
    }
}