using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsApi.Services
{
    public interface ITransactionService
    {
        decimal GetAccountTotal(string accountId, TransactionCurrency currency);
        decimal GetMerchantTotal(string merchantName, TransactionCurrency currency);
        decimal GetCurrencyTotal(TransactionCurrency currency);
        decimal GetReferenceTotal(string reference, TransactionCurrency currency);
        IReadOnlyList<Transaction> GetTransactions(
            string? accountId = null,
            string? merchantName = null,
            string? reference = null,
            TransactionCurrency? currency = null,
            TransactionType? type = null,
            TransactionStatus? status = null);
    }
}
