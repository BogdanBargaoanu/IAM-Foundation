using TransactionsApi.Mock;
using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsApi.Services
{
    public sealed class TransactionService : ITransactionService
    {
        public decimal GetBalanceForCurrency(
            TransactionCurrency currency,
            SearchCriteria searchBy = SearchCriteria.None,
            string? searchValue = null)
        {
            var result = 0m;
            var relevant = MockData.Transactions.Where(tr => tr.Currency == currency);
            if (searchBy != SearchCriteria.None && !string.IsNullOrEmpty(searchValue))
            {
                switch (searchBy)
                {
                    case SearchCriteria.Account:
                        relevant = relevant.Where(tr => tr.AccountId == searchValue);
                        break;
                    case SearchCriteria.Merchant:
                        relevant = relevant.Where(tr => tr.MerchantName == searchValue);
                        break;
                    case SearchCriteria.Reference:
                        relevant = relevant.Where(tr => tr.Reference == searchValue);
                        break;
                    default:
                        throw new ArgumentException($"Invalid search parameter: {searchBy}", nameof(searchBy));
                }
            }
            if (!relevant.Any()) return result;

            result = relevant.Sum(r => r.Amount);
            return result;
        }
        public decimal GetAccountTotal(string accountId, TransactionCurrency currency)
        {
            var result = 0m;
            var relevant = MockData.Transactions.Where(tr => tr.AccountId == accountId && tr.Currency == currency);
            if (!relevant.Any()) return result;

            result = relevant.Sum(r => r.Amount);
            return result;
        }

        public IReadOnlyDictionary<TransactionCurrency, decimal> GetAccountTotal(string accountId)
        {
            var totals = Enum.GetValues<TransactionCurrency>()
                .ToDictionary(
                    currency => currency,
                    currency => GetAccountTotal(accountId, currency)
                );
            return totals;
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
