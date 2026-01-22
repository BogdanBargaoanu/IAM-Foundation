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
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is null)
            {
                throw new InvalidOperationException("HttpContext is not available. This provider can only be used within a HTTP request context");
            }
            var token = await httpContext.GetUserAccessTokenAsync(null, cancellationToken);
            return Convert.ToString(token?.Token?.AccessToken) ?? string.Empty;
        }
    }
}
