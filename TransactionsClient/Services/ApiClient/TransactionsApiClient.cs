using System.Text.Json;
using TransactionsClient.Services.HttpClientFactory;
using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsClient.Services.ApiClient
{
    public class TransactionsApiClient : ITransactionsApiClient
    {
        private readonly IAuthenticatedHttpClientFactory _clientFactory;
        private readonly ILogger<TransactionsApiClient> _logger;

        public TransactionsApiClient(
            IAuthenticatedHttpClientFactory clientFactory,
            ILogger<TransactionsApiClient> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<decimal> GetAccountTotalV1Async(string accountId, TransactionCurrency currency)
        {
            var client = await _clientFactory.CreateClientAsync();
            _logger.LogInformation("Fetching V1 account total for: {AccountId}, currency: {Currency}", accountId, currency);

            var response = await client.GetAsync($"/api/v1/Transaction/account/{accountId}?currency={(int)currency}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var total = JsonSerializer.Deserialize<decimal>(content);
            return total;
        }

        public async Task<IReadOnlyDictionary<string, decimal>> GetAccountTotalV2Async(string accountId)
        {
            var client = await _clientFactory.CreateClientAsync();
            _logger.LogInformation("Fetching V2 account total for: {AccountId}", accountId);

            var response = await client.GetAsync($"/api/v2/Transaction/account/{accountId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var totals = JsonSerializer.Deserialize<Dictionary<string, decimal>>(content);
            return totals;
        }

        public async Task<decimal> GetCurrencyTotalAsync(TransactionCurrency currency)
        {
            var client = await _clientFactory.CreateClientAsync();
            _logger.LogInformation("Fetching total for currency: {Currency}", currency);

            var response = await client.GetAsync($"/api/v1/Transaction/currency/{(int)currency}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var total = JsonSerializer.Deserialize<decimal>(content);
            return total;
        }

        public async Task<decimal> GetMerchantTotalAsync(string merchantName, TransactionCurrency currency)
        {
            var client = await _clientFactory.CreateClientAsync();
            _logger.LogInformation("Fetching total for merchant: {MerchantName}, currency: {Currency}", merchantName, currency);

            var response = await client.GetAsync($"/api/v1/Transaction/merchant/{merchantName}?currency={(int)currency}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var total = JsonSerializer.Deserialize<decimal>(content);
            return total;
        }

        public async Task<decimal> GetReferenceTotalAsync(string reference, TransactionCurrency currency)
        {
            var client = await _clientFactory.CreateClientAsync();
            _logger.LogInformation("Fetching total for reference: {Reference}, currency: {Currency}", reference, currency);

            var response = await client.GetAsync($"/api/v1/Transaction/reference/{reference}?currency={(int)currency}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var total = JsonSerializer.Deserialize<decimal>(content);
            return total;
        }

        public async Task<IReadOnlyList<Transaction>> GetTransactionsAsync(
            string? accountId = null,
            string? merchantName = null,
            string? reference = null,
            TransactionCurrency? currency = null,
            TransactionType? type = null,
            TransactionStatus? status = null)
        {
            var client = await _clientFactory.CreateClientAsync();
            _logger.LogInformation("Fetching transactions with filters");

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(accountId)) queryParams.Add($"accountId={accountId}");
            if (!string.IsNullOrEmpty(merchantName)) queryParams.Add($"merchantName={merchantName}");
            if (!string.IsNullOrEmpty(reference)) queryParams.Add($"reference={reference}");
            if (currency.HasValue) queryParams.Add($"currency={(int)currency}");
            if (type.HasValue) queryParams.Add($"type={(int)type}");
            if (status.HasValue) queryParams.Add($"status={(int)status}");

            var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var response = await client.GetAsync($"/api/v1/Transaction/transactions{query}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<List<Transaction>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return transactions ?? new List<Transaction>();
        }
    }
}
