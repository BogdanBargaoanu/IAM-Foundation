namespace TransactionsApiClient.Services.Auth.Providers
{
    public interface IAccessTokenProvider
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    }
}
