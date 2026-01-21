using System.Text.Json;
using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsApiClient.Services.ApiClient
{
    public sealed class TransactionsApiClient : ITransactionsApiClient
    {
        private readonly HttpClient _httpClient;

        public TransactionsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CheckHealthyAsync()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            using var response = await _httpClient.GetAsync("/api/health", cts.Token);
            return response.IsSuccessStatusCode;
        }

        public Task<decimal> GetBalanceForCurrencyAsync(
            TransactionCurrency currency,
            SearchCriteria searchBy = SearchCriteria.None,
            string? searchValue = null)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetAccountTotalAsync(string accountId, TransactionCurrency currency)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Transaction>> GetTransactionsAsync(
                string? accountId = null,
                string? merchantName = null,
                string? reference = null,
                TransactionCurrency? currency = null,
                TransactionType? type = null,
                TransactionStatus? status = null)
        {
            using var response = await _httpClient.GetAsync("/api/v1/transactions");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<List<Transaction>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return transactions ?? [];
        }
    }
}
