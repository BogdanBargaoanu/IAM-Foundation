using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsApiClient.Services.ApiClient
{
    public interface ITransactionsApiClient
    {
        Task<bool> CheckHealthyAsync();

        Task<decimal> GetAmountsForCurrencyAsync(
            TransactionCurrency currency,
            SearchCriteria searchBy = SearchCriteria.None,
            string? searchValue = null);

        Task<IReadOnlyDictionary<string, decimal>> GetAccountTotalAsync(string accountId);

        Task<IReadOnlyList<Transaction>> GetTransactionsAsync(
            string? accountId = null,
            string? merchantName = null,
            string? reference = null,
            TransactionCurrency? currency = null,
            TransactionType? type = null,
            TransactionStatus? status = null);
    }
}
