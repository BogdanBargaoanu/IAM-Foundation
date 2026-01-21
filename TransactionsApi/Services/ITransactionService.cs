using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsApi.Services
{
    public interface ITransactionService
    {
        decimal GetBalanceForCurrency(
            TransactionCurrency currency,
            SearchCriteria searchBy = SearchCriteria.None,
            string? searchValue = null);
        decimal GetAccountTotal(string accountId, TransactionCurrency currency);

        IReadOnlyDictionary<TransactionCurrency, decimal> GetAccountTotal(string accountId);
        IReadOnlyList<Transaction> GetTransactions(
            string? accountId = null,
            string? merchantName = null,
            string? reference = null,
            TransactionCurrency? currency = null,
            TransactionType? type = null,
            TransactionStatus? status = null);
    }
}
