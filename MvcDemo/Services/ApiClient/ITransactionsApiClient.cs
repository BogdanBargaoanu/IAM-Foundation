using TransactionsLibrary.Models;

namespace MvcDemo.Services.ApiClient
{
    public interface ITransactionsApiClient
    {
        Task<bool> CheckHealthy();
        Task<IReadOnlyList<Transaction>> GetTransactionsAsync();
    }
}
