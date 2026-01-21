using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Http;

namespace TransactionsApiClient.Services.Auth.Providers
{
    public sealed class HttpContextAccessTokenProvider : IAccessTokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextAccessTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            var token = await _httpContextAccessor.HttpContext?.GetUserAccessTokenAsync();
            return Convert.ToString(token?.Token?.AccessToken) ?? string.Empty;
        }
    }
}
