using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsClient.Services.ApiClient
{
    public interface ITransactionsApiClient
    {
        Task<decimal> GetAccountTotalV1Async(string accountId, TransactionCurrency currency);
        Task<decimal> GetMerchantTotalAsync(string merchantName, TransactionCurrency currency);
        Task<decimal> GetCurrencyTotalAsync(TransactionCurrency currency);
        Task<decimal> GetReferenceTotalAsync(string reference, TransactionCurrency currency);
        Task<IReadOnlyList<Transaction>> GetTransactionsAsync(
            string? accountId = null,
            string? merchantName = null,
            string? reference = null,
            TransactionCurrency? currency = null,
            TransactionType? type = null,
            TransactionStatus? status = null);
        Task<IReadOnlyDictionary<string, decimal>> GetAccountTotalV2Async(string accountId);
    }
}
