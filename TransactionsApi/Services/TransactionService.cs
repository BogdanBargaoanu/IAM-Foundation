using Microsoft.EntityFrameworkCore;
using TransactionsApi.Data;
using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsApi.Services
{
    public sealed class TransactionService : ITransactionService
    {
        private readonly TransactionsDbContext _dbContext;
        public TransactionService(TransactionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<decimal> GetBalanceForCurrencyAsync(
            TransactionCurrency currency,
            SearchCriteria searchBy = SearchCriteria.None,
            string? searchValue = null)
        {
            IQueryable<Transaction> query = _dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.Currency == currency);
            if (searchBy != SearchCriteria.None && !string.IsNullOrEmpty(searchValue))
            {
                switch (searchBy)
                {
                    case SearchCriteria.Account:
                        query = query.Where(tr => tr.AccountId == searchValue);
                        break;
                    case SearchCriteria.Merchant:
                        query = query.Where(tr => tr.MerchantName == searchValue);
                        break;
                    case SearchCriteria.Reference:
                        query = query.Where(tr => tr.Reference == searchValue);
                        break;
                    default:
                        throw new ArgumentException($"Invalid search parameter: {searchBy}", nameof(searchBy));
                }
            }

            return await query.SumAsync(t => t.Amount);
        }
        public async Task<decimal> GetAccountTotalAsync(string accountId, TransactionCurrency currency)
        {
            return await _dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.AccountId == accountId && t.Currency == currency)
                .SumAsync(t => t.Amount);
        }

        public async Task<IReadOnlyDictionary<TransactionCurrency, decimal>> GetAccountTotalAsync(string accountId)
        {
            var totals = new Dictionary<TransactionCurrency, decimal>();

            foreach (var currency in Enum.GetValues<TransactionCurrency>())
            {
                totals[currency] = await GetAccountTotalAsync(accountId, currency);
            }

            return totals;
        }

        public async Task<IReadOnlyList<Transaction>> GetTransactionsAsync(
            string? accountId = null,
            string? merchantName = null,
            string? reference = null,
            TransactionCurrency? currency = null,
            TransactionType? type = null,
            TransactionStatus? status = null)
        {
            IQueryable<Transaction> query = _dbContext.Transactions.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(accountId)) query = query.Where(t => t.AccountId == accountId);
            if (!string.IsNullOrWhiteSpace(merchantName)) query = query.Where(t => t.MerchantName == merchantName);
            if (!string.IsNullOrWhiteSpace(reference)) query = query.Where(t => t.Reference == reference);
            if (currency.HasValue) query = query.Where(t => t.Currency == currency.Value);
            if (type.HasValue) query = query.Where(t => t.Type == type.Value);
            if (status.HasValue) query = query.Where(t => t.Status == status.Value);

            return await query
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }
    }
}
