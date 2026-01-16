namespace TransactionsClient.Services.HttpClientFactory
{
    public interface IAuthenticatedHttpClientFactory
    {
        Task<HttpClient> CreateClientAsync(string accessToken);
    }
}
