using Duende.AccessTokenManagement.OpenIdConnect;

namespace TransactionsClient.Services.HttpClientFactory
{
    public interface IAuthenticatedHttpClientFactory
    {
        Task<HttpClient> CreateClientAsync(UserToken accessToken);
    }
}
