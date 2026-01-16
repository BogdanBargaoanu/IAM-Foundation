using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Text.Json;
using TransactionsClient.Services.HttpClientFactory;
using TransactionsLibrary.Models;

namespace MvcDemo.Services.ApiClient
{
    public class TransactionsApiClient : ITransactionsApiClient
    {
        private readonly IAuthenticatedHttpClientFactory _clientFactory;
        private readonly ILogger<TransactionsApiClient> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionsApiClient(
            IAuthenticatedHttpClientFactory clientFactory,
            IConfiguration configuration,
            ILogger<TransactionsApiClient> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CheckHealthy()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_configuration["TransactionsApi:BaseUrl"] ?? "https://localhost:7001");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromSeconds(1);
            _logger.LogInformation("Checking API health");

            var response = await client.GetAsync("/api/health");
            _logger.LogInformation("API Service is {status}", response.IsSuccessStatusCode ? "healthy" : "unhealthy");
            return response.IsSuccessStatusCode;
        }

        public async Task<IReadOnlyList<Transaction>> GetTransactionsAsync()
        {
            var accessToken = await _httpContextAccessor.HttpContext?.GetTokenAsync("access_token") ?? string.Empty;

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("No access token found in HttpContext");
                throw new InvalidOperationException("User is not authenticated or token is missing");
            }

            var client = await _clientFactory.CreateClientAsync(accessToken);
            _logger.LogInformation("Fetching available transactions");

            var response = await client.GetAsync($"/api/v1/Transaction/transactions");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<List<Transaction>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return transactions ?? new List<Transaction>();
        }
    }
}