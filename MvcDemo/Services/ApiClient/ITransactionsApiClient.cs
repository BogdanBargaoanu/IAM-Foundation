using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace MvcDemo.Services.ApiClient
{
    public interface ITransactionsApiClient
    {
        Task<bool> CheckHealthy();
        Task<IReadOnlyList<Transaction>> GetTransactionsAsync(
            string? accountId = null,
            string? merchantName = null,
            string? reference = null,
            TransactionCurrency? currency = null,
            TransactionType? type = null,
            TransactionStatus? status = null);
    }
}
