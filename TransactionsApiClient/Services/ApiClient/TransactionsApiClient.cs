using Microsoft.AspNetCore.WebUtilities;
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
            using var response = await _httpClient.GetAsync("/api/health");
            return response.IsSuccessStatusCode;
        }

        public async Task<decimal> GetAmountsForCurrencyAsync(
            TransactionCurrency currency,
            SearchCriteria searchBy = SearchCriteria.None,
            string? searchValue = null)
        {
            var query = new Dictionary<string, string?>
            {
                ["currency"] = ((int)currency).ToString()
            };

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query["searchBy"] = ((int)searchBy).ToString();
                query["searchValue"] = searchValue;
            }

            var url = QueryHelpers.AddQueryString("/api/v1/transactions/amounts", query);

            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var total = JsonSerializer.Deserialize<decimal>(content);
            return total;
        }

        public async Task<IReadOnlyDictionary<string, decimal>> GetAccountTotalAsync(string accountId)
        {
            using var response = await _httpClient.GetAsync($"/api/v2/transactions/accounts/{accountId}");
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
            var query = new Dictionary<string, string?>();

            if (!string.IsNullOrWhiteSpace(accountId)) query["accountId"] = accountId;
            if (!string.IsNullOrWhiteSpace(merchantName)) query["merchantName"] = merchantName;
            if (!string.IsNullOrWhiteSpace(reference)) query["reference"] = reference;
            if (currency.HasValue) query["currency"] = ((int)currency.Value).ToString();
            if (type.HasValue) query["type"] = ((int)type.Value).ToString();
            if (status.HasValue) query["status"] = ((int)status.Value).ToString();

            var url = QueryHelpers.AddQueryString("/api/v1/transactions", query);

            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<List<Transaction>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return transactions ?? [];
        }
    }
}
