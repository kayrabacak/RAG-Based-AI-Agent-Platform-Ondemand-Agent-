using Blazored.LocalStorage;
using OndemandAgent.UI.Dtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OndemandAgent.UI.Services
{
    public class AnalyticsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public AnalyticsService(HttpClient httpClient, IConfiguration configuration, ILocalStorageService localStorage)
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

        public async Task<AnalyticsSummaryDto?> GetSummaryAsync()
        {
            try
            {
                await AddAuthorizationHeader();
                return await _httpClient.GetFromJsonAsync<AnalyticsSummaryDto>("/api/analytics/summary");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Analytics Error: {ex.Message}");
                return null;
            }
        }

        public async Task<AnalyticsSummaryDto?> GetEventDetailsAsync(Guid eventId)
        {
            try
            {
                await AddAuthorizationHeader();
                return await _httpClient.GetFromJsonAsync<AnalyticsSummaryDto>($"/api/analytics/events/{eventId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Analytics Details Error: {ex.Message}");
                return null;
            }
        }
    }
}
