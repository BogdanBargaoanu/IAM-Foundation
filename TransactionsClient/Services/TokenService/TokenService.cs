using Duende.IdentityModel.Client;

namespace TransactionsClient.Services.TokenService
{
    public class TokenService : ITokenService
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
        public async Task<string> GetAccesTokenAsync()
        {
            // Return cached token if still valid
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiration)
            {
                return _cachedToken;
            }

            var client = _httpClientFactory.CreateClient();

            var disco = await client.GetDiscoveryDocumentAsync(_configuration["IdentityServer:Authority"]);
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
            });

            if (tokenResponse.IsError)
            {
                _logger.LogError("Error retrieving access token: {Error}", tokenResponse.Error);
                throw new Exception("Error retrieving access token", tokenResponse.Exception);
            }

            _cachedToken = tokenResponse.AccessToken;
            // Refresh the token 30 seconds before it expires
            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 30);
            _logger.LogInformation("Succesfully retrieved an access token from the IdentityServer: {Token}", _cachedToken);

            return _cachedToken;
        }
    }
}
