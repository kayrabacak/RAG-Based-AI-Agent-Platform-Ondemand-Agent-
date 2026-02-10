using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Forms;
using OndemandAgent.UI.Dtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OndemandAgent.UI.Services
{
    public class EventService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public EventService(HttpClient httpClient, IConfiguration configuration, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            var baseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(baseUrl ?? "http://localhost:5000");
        }

        private async Task AddAuthorizationHeader()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<EventDto>> GetMyEvents()
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("/api/events");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<EventDto>>() ?? new List<EventDto>();
            }
            return new List<EventDto>();
        }

        public async Task<bool> CreateEvent(CreateEventRequest request)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("/api/events", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<EventDto> GetEventById(string id)
        {
            await AddAuthorizationHeader();
            if (string.IsNullOrEmpty(id)) return null;
            return await _httpClient.GetFromJsonAsync<EventDto>($"/api/events/{id}");
        }

        public async Task<bool> UploadDocument(string eventId, IBrowserFile file)
        {
            await AddAuthorizationHeader();
            
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(eventId), "EventId");
            
            var fileContent = new StreamContent(file.OpenReadStream(10 * 1024 * 1024));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            
            content.Add(fileContent, "File", file.Name);

            var response = await _httpClient.PostAsync("/api/documents/upload", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateEvent(Guid id, CreateEventRequest request)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"/api/events/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ChatLogDto>> GetEventLogs(string eventId)
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<ChatLogDto>>($"/api/events/{eventId}/logs") 
                   ?? new List<ChatLogDto>();
        }

        public async Task<bool> DeleteEvent(Guid id)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"/api/events/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}