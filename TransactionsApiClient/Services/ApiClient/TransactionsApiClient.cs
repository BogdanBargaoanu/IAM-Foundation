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

        public async Task<decimal> GetBalanceForCurrencyAsync(
            TransactionCurrency currency,
            SearchCriteria searchBy = SearchCriteria.None,
            string? searchValue = null)
        {
            var queryParams = $"currency={currency}";
            if (!string.IsNullOrEmpty(searchValue)) queryParams += $"?searchBy={searchBy}?searchValue={searchValue}";

            using var response = await _httpClient.GetAsync($"/api/v1/transactions/amounts/{queryParams}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var total = JsonSerializer.Deserialize<decimal>(content);
            return total;
        }

        public async Task<IReadOnlyDictionary<string, decimal>> GetAccountTotalAsync(string accountId)
        {
            using var response = await _httpClient.GetAsync($"/api/v1/transactions/accounts/{accountId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var totals = JsonSerializer.Deserialize<Dictionary<string, decimal>>(content);
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
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(accountId)) queryParams.Add($"accountId={accountId}");
            if (!string.IsNullOrEmpty(merchantName)) queryParams.Add($"merchantName={merchantName}");
            if (!string.IsNullOrEmpty(reference)) queryParams.Add($"reference={reference}");
            if (currency.HasValue) queryParams.Add($"currency={(int)currency}");
            if (type.HasValue) queryParams.Add($"type={(int)type}");
            if (status.HasValue) queryParams.Add($"status={(int)status}");

            var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            using var response = await _httpClient.GetAsync($"/api/v1/transactions{query}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<List<Transaction>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return transactions ?? [];
        }
    }
}
