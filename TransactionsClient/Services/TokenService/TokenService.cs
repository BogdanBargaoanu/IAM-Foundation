using Duende.IdentityModel.Client;
using TransactionsApiClient.Services.Auth.Providers;

namespace TransactionsClient.Services.TokenService
{
    public class TokenService : IAccessTokenProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        private string? _cachedToken;
        private DateTime _tokenExpiration;

        public TokenService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<TokenService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            // Return cached token if still valid
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiration)
            {
                return _cachedToken;
            }

            var client = _httpClientFactory.CreateClient();

            var disco = await client.GetDiscoveryDocumentAsync(_configuration["IdentityServer:Authority"], cancellationToken);
            if (disco.IsError)
            {
                _logger.LogError("Error retrieving discovery document: {Error}", disco.Error);
                throw new Exception("Error retrieving discovery document", disco.Exception);
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _configuration["IdentityServer:ClientId"],
                ClientSecret = _configuration["IdentityServer:ClientSecret"],
                Scope = _configuration["IdentityServer:Scope"]
            }, cancellationToken);

            if (tokenResponse.IsError)
            {
                _logger.LogError("Error retrieving access token: {Error}", tokenResponse.Error);
                throw new Exception("Error retrieving access token", tokenResponse.Exception);
            }

            _cachedToken = tokenResponse.AccessToken;

            // Refresh the token 30 seconds before it expires
            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 30);
            _logger.LogInformation("Successfully retrieved an access token from the IdentityServer: {Token}", _cachedToken);

            return _cachedToken ?? string.Empty;
        }
    }
}
