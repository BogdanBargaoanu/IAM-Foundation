using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Text;
using System.Text.Json;
using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsApiClient.Services.ApiClient
{
    public sealed class TransactionsApiClient : ITransactionsApiClient
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

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
                TransactionStatus? status = null,
                int page = 1,
                int pageSize = 10)
        {
            var query = new Dictionary<string, string?>();

            if (!string.IsNullOrWhiteSpace(accountId)) query["accountId"] = accountId;
            if (!string.IsNullOrWhiteSpace(merchantName)) query["merchantName"] = merchantName;
            if (!string.IsNullOrWhiteSpace(reference)) query["reference"] = reference;
            if (currency.HasValue) query["currency"] = ((int)currency.Value).ToString();
            if (type.HasValue) query["type"] = ((int)type.Value).ToString();
            if (status.HasValue) query["status"] = ((int)status.Value).ToString();
            query["page"] = page.ToString();
            query["pageSize"] = pageSize.ToString();

            var url = QueryHelpers.AddQueryString("/api/v1/transactions", query);

            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<List<Transaction>>(content, JsonOptions);

            return transactions ?? [];
        }

        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            using var response = await _httpClient.GetAsync($"/api/v2/transactions/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transaction = JsonSerializer.Deserialize<Transaction>(content, JsonOptions);

            return transaction!;
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            using var content = new StringContent(
                JsonSerializer.Serialize(transaction, JsonOptions),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.PostAsync($"/api/v2/transactions", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdTransaction = JsonSerializer.Deserialize<Transaction>(responseContent, JsonOptions);

            return createdTransaction!;
        }

        public async Task<Transaction> UpdateTransactionAsync(Guid id, Transaction transaction)
        {
            using var content = new StringContent(
                JsonSerializer.Serialize(transaction, JsonOptions),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.PutAsync($"/api/v2/transactions/{id}", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var updatedTransaction = JsonSerializer.Deserialize<Transaction>(responseContent, JsonOptions);

            return updatedTransaction!;
        }

        public async Task<bool> DeleteTransactionAsync(Guid id)
        {
            using var response = await _httpClient.DeleteAsync($"/api/v2/transactions/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
