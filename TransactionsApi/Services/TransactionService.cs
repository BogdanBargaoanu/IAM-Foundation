using TransactionsApi.Constants;
using TransactionsApi.Mock;
using TransactionsApi.Models;

namespace TransactionsApi.Services
{
    public sealed class TransactionService : ITransactionService
    {
        public decimal GetAccountTotal(string accountId, TransactionCurrency currency)
        {
            var result = 0m;
            var relevant = MockData.Transactions.Where(tr => tr.AccountId == accountId && tr.Currency == currency);
            if (!relevant.Any()) return result;

            result = relevant.Sum(r => r.Amount);
            return result;
        }

        public decimal GetCurrencyTotal(TransactionCurrency currency)
        {
            var result = 0m;
            var relevant = MockData.Transactions.Where(tr => tr.Currency == currency);
            if (!relevant.Any()) return result;

            result = relevant.Sum(r => r.Amount);
            return result;
        }

        public decimal GetMerchantTotal(string merchantName, TransactionCurrency currency)
        {
            var result = 0m;
            var relevant = MockData.Transactions.Where(tr => tr.MerchantName == merchantName);
            if (!relevant.Any()) return result;

            result = relevant.Sum(r => r.Amount);
            return result;
        }

        public decimal GetReferenceTotal(string reference, TransactionCurrency currency)
        {
            var result = 0m;
            var relevant = MockData.Transactions.Where(tr => tr.Reference == reference);
            if (!relevant.Any()) return result;

            result = relevant.Sum(r => r.Amount);
            return result;
        }

        public IReadOnlyList<Transaction> GetTransactions(
            string? accountId = null,
            string? merchantName = null,
            string? reference = null,
            TransactionCurrency? currency = null,
            TransactionType? type = null,
            TransactionStatus? status = null)
        {
            var query = MockData.Transactions.AsEnumerable();

            if (!string.IsNullOrEmpty(accountId))
            {
                query = query.Where(tr => tr.AccountId == accountId);
            }

            if (!string.IsNullOrEmpty(merchantName))
            {
                query = query.Where(tr => tr.MerchantName == merchantName);
            }

            if (!string.IsNullOrEmpty(reference))
            {
                query = query.Where(tr => tr.Reference == reference);
            }

            if (currency.HasValue)
            {
                query = query.Where(tr => tr.Currency == currency.Value);
            }

            if (type.HasValue)
            {
                query = query.Where(tr => tr.Type == type.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(tr => tr.Status == status.Value);
            }

            return query.ToArray();
        }
    }
}
