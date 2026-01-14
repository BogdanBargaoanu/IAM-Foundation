using System.Net.Http.Headers;
using TransactionsClient.Services.TokenService;

namespace TransactionsClient.Services.HttpClientFactory
{
    public class AuthenticatedHttpClientFactory : IAuthenticatedHttpClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticatedHttpClientFactory> _logger;

        public AuthenticatedHttpClientFactory(
            IHttpClientFactory httpClientFactory,
            ITokenService tokenService,
            IConfiguration configuration,
            ILogger<AuthenticatedHttpClientFactory> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<HttpClient> CreateClientAsync()
        {
            var client = _httpClientFactory.CreateClient();

            var accessToken = await _tokenService.GetAccesTokenAsync();

            client.BaseAddress = new Uri(_configuration["TransactionsApi:BaseUrl"] ?? "https://localhost:6001");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _logger.LogInformation("Successfully created an authenticated HttpClient");

            return client;
        }
    }
}
