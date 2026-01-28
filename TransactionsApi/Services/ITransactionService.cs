using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsApi.Services
{
    public interface ITransactionService
    {
        Task<decimal> GetBalanceForCurrencyAsync(
            TransactionCurrency currency,
            SearchCriteria searchBy = SearchCriteria.None,
            string? searchValue = null);

        Task<decimal> GetAccountTotalAsync(string accountId, TransactionCurrency currency);

        Task<IReadOnlyDictionary<TransactionCurrency, decimal>> GetAccountTotalAsync(string accountId);

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
