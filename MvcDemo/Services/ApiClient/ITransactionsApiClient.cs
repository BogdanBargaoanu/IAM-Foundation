using TransactionsLibrary.Models;

namespace MvcDemo.Services.ApiClient
{
    public interface ITransactionsApiClient
    {
        void InjectAccessToken(string accessToken);
        Task<bool> CheckHealthy();
        Task<IReadOnlyList<Transaction>> GetTransactionsAsync();
    }
}
