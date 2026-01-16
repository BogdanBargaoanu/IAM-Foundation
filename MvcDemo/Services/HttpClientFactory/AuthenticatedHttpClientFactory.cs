using System.Net.Http.Headers;

namespace TransactionsClient.Services.HttpClientFactory
{
    public class AuthenticatedHttpClientFactory : IAuthenticatedHttpClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticatedHttpClientFactory> _logger;

        public AuthenticatedHttpClientFactory(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<AuthenticatedHttpClientFactory> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<HttpClient> CreateClientAsync(string accessToken)
        {
            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(_configuration["TransactionsApi:BaseUrl"] ?? "https://localhost:7001");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _logger.LogInformation("Successfully created an authenticated HttpClient");

            return client;
        }
    }
}
