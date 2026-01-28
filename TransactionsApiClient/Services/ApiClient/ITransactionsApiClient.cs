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

        Task<int> GetCountAsync(
            string? accountId = null,
            string? merchantName = null,
            string? reference = null,
            TransactionCurrency? currency = null,
            TransactionType? type = null,
            TransactionStatus? status = null);

        Task<IReadOnlyList<Transaction>> GetTransactionsAsync(
            string? accountId = null,
            string? merchantName = null,
            string? reference = null,
            TransactionCurrency? currency = null,
            TransactionType? type = null,
            TransactionStatus? status = null,
            int page = Pagination.DefaultPageIndex,
            int pageSize = Pagination.DefaultPageSize);

        Task<Transaction?> GetByIdAsync(Guid id);

        Task<Transaction> CreateTransactionAsync(Transaction transaction);

        Task<Transaction> UpdateTransactionAsync(Guid id, Transaction transaction);

        Task<bool> DeleteTransactionAsync(Guid id);
    }
}
