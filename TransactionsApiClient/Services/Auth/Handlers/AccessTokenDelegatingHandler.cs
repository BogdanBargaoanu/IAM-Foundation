using System.Net.Http.Headers;
using TransactionsApiClient.Services.Auth.Providers;

namespace TransactionsApiClient.Services.Auth.Handlers
{
    public sealed class AccessTokenDelegatingHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider _tokenProvider;

        public AccessTokenDelegatingHandler(IAccessTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
