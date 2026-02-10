using Microsoft.Extensions.Configuration;
using OndemandAgent.Web.Dtos;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OndemandAgent.Web.Services
{
    public interface IAIService
    {
        Task<bool> IndexDocumentAsync(Guid eventId, Guid documentId, string filePath);
        Task<string> ChatAsync(Guid eventId, string question);
    }

    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public AIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["AISettings:BaseUrl"] ?? "http://localhost:8000";
        }

        public async Task<bool> IndexDocumentAsync(Guid eventId, Guid documentId, string filePath)
        {
            var requestData = new AIIndexRequest
            {
                EventId = eventId.ToString(),
                DocumentId = documentId.ToString(),
                FilePath = filePath
            };

            var json = JsonSerializer.Serialize(requestData);
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/index-document", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI Service Error: {ex.Message}");
                return false;
            }
        }

        public async Task<string> ChatAsync(Guid eventId, string question)
        {
            var requestData = new AIChatRequest
            {
                EventId = eventId.ToString(),
                Question = question
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/chat", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<AIChatResponse>(responseString);
                    return result?.Answer ?? "Cevap alınamadı.";
                }
                return "AI Servisine erişilemedi.";
            }
            catch (Exception ex)
            {
                return $"AI Hatası: {ex.Message}";
            }
        }
    }
}