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
        Task<IReadOnlyList<Transaction>> GetTransactionsAsync(
            string? accountId = null,
            string? merchantName = null,
            string? reference = null,
            TransactionCurrency? currency = null,
            TransactionType? type = null,
            TransactionStatus? status = null);
    }
}
