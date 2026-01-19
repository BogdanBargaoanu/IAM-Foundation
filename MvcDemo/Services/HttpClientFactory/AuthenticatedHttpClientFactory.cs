using Duende.AccessTokenManagement.OpenIdConnect;
using Duende.IdentityModel.Client;
using System.Net.Http.Headers;

namespace TransactionsClient.Services.HttpClientFactory
{
    public class AuthenticatedHttpClientFactory : IAuthenticatedHttpClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticatedHttpClientFactory> _logger;
        private readonly IHostEnvironment _environment;

        public AuthenticatedHttpClientFactory(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<AuthenticatedHttpClientFactory> logger,
            IHostEnvironment environment)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _environment = environment;
        }
        public async Task<HttpClient> CreateClientAsync(UserToken token)
        {
            HttpClient client;
            if (_environment.IsDevelopment())
            {
                // Skip certificate validation
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                client = new HttpClient(handler);
            }
            else
            {
                client = _httpClientFactory.CreateClient();
            }

            client.BaseAddress = new Uri(_configuration["TransactionsApi:BaseUrl"] ?? "https://localhost:7001");
            client.SetBearerToken(token.AccessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _logger.LogInformation("Successfully created an authenticated HttpClient");

            return client;
        }
    }
}
