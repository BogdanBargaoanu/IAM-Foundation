using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TransactionsApiClient.Services.ApiClient;
using TransactionsClient.Services.Utils;
using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsClient.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionsApiClient _apiClient;
        private readonly ILogger<TransactionsController> _logger;
        private readonly CurlCommandBuilder _curlBuilder;
        private const string AmountsEndpoint = "/api/v1/transactions/amounts";
        private const string AccountTotalEndpoint = "api/v2/transactions/accounts";

        public TransactionsController(
            ITransactionsApiClient apiClient,
            ILogger<TransactionsController> logger,
            CurlCommandBuilder curlBuilder)
        {
            _apiClient = apiClient;
            _logger = logger;
            _curlBuilder = curlBuilder;
        }
        public IActionResult Index()
        {
            PopulateDropdowns();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetAccountTotalV1(string accountId, TransactionCurrency currency)
        {
            ViewBag.CurlCommand = _curlBuilder.BuildCommand(
                AmountsEndpoint,
                new()
                {
                    ["currency"] = ((int)currency).ToString(),
                    ["searchBy"] = ((int)SearchCriteria.Account).ToString(),
                    ["searchValue"] = accountId
                });

            try
            {
                var total = await _apiClient.GetAmountsForCurrencyAsync(currency, SearchCriteria.Account, accountId);
                ViewBag.Result = $"Account Total : {total:C} {currency}";
                ViewBag.ResultType = "success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account total V1");
                ViewBag.Result = $"Error: {ex.Message}";
                ViewBag.ResultType = "error";
            }

            PopulateDropdowns();
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> GetAccountTotalV2(string accountId)
        {
            ViewBag.CurlCommand = _curlBuilder.BuildCommand($"{AccountTotalEndpoint}/{accountId}");

            try
            {
                var totals = await _apiClient.GetAccountTotalAsync(accountId);
                ViewBag.Result = string.Join("<br/>", totals.Select(t => $"{t.Key}: {t.Value:C}"));
                ViewBag.ResultType = "success";
                ViewBag.IsHtml = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account total V2");
                ViewBag.Result = $"Error: {ex.Message}";
                ViewBag.ResultType = "error";
            }

            PopulateDropdowns();
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> GetMerchantTotal(string merchantName, TransactionCurrency currency)
        {
            ViewBag.CurlCommand = _curlBuilder.BuildCommand(
                AmountsEndpoint,
                new()
                {
                    ["currency"] = ((int)currency).ToString(),
                    ["searchBy"] = ((int)SearchCriteria.Merchant).ToString(),
                    ["searchValue"] = merchantName
                });

            try
            {
                var total = await _apiClient.GetAmountsForCurrencyAsync(currency, SearchCriteria.Merchant, merchantName);
                ViewBag.Result = $"Merchant Total: {total:C} {currency}";
                ViewBag.ResultType = "success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching merchant total");
                ViewBag.Result = $"Error: {ex.Message}";
                ViewBag.ResultType = "error";
            }

            PopulateDropdowns();
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> GetCurrencyTotal(TransactionCurrency currency)
        {
            ViewBag.CurlCommand = _curlBuilder.BuildCommand(AmountsEndpoint,
                new()
                {
                    ["currency"] = ((int)currency).ToString()
                });

            try
            {
                var total = await _apiClient.GetAmountsForCurrencyAsync(currency);
                ViewBag.Result = $"Currency Total: {total:C} {currency}";
                ViewBag.ResultType = "success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching currency total");
                ViewBag.Result = $"Error: {ex.Message}";
                ViewBag.ResultType = "error";
            }

            PopulateDropdowns();
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> GetReferenceTotal(string reference, TransactionCurrency currency)
        {
            ViewBag.CurlCommand = _curlBuilder.BuildCommand(
                AmountsEndpoint,
                new()
                {
                    ["currency"] = ((int)currency).ToString(),
                    ["searchBy"] = ((int)SearchCriteria.Reference).ToString(),
                    ["searchValue"] = reference
                });

            try
            {
                var total = await _apiClient.GetAmountsForCurrencyAsync(currency, SearchCriteria.Reference, reference);
                ViewBag.Result = $"Reference Total: {total:C} {currency}";
                ViewBag.ResultType = "success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reference total");
                ViewBag.Result = $"Error: {ex.Message}";
                ViewBag.ResultType = "error";
            }

            PopulateDropdowns();
            return View("Index");
        }
        public async Task<IActionResult> Transactions(
            string? accountId,
            string? merchantName,
            string? reference,
            TransactionCurrency? currency,
            TransactionType? type,
            TransactionStatus? status)
        {
            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accountId)) queryParams["accountId"] = accountId;
            if (!string.IsNullOrEmpty(merchantName)) queryParams["merchantName"] = merchantName;
            if (!string.IsNullOrEmpty(reference)) queryParams["reference"] = reference;
            if (currency.HasValue) queryParams["currency"] = ((int)currency).ToString();
            if (type.HasValue) queryParams["type"] = ((int)type).ToString();
            if (status.HasValue) queryParams["status"] = ((int)status).ToString();

            ViewBag.CurlCommand = _curlBuilder.BuildCommand("/api/v1/transactions", queryParams);

            try
            {
                var transactions = await _apiClient.GetTransactionsAsync(
                    accountId, merchantName, reference, currency, type, status);

                PopulateDropdowns();
                return View(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transactions");
                ViewBag.Error = $"Error: {ex.Message}";
                PopulateDropdowns();
                return View(new List<Transaction>());
            }
        }
        private void PopulateDropdowns()
        {
            ViewBag.Currencies = Enum.GetValues<TransactionCurrency>()
                .Select(c => new SelectListItem
                {
                    Value = ((int)c).ToString(),
                    Text = c.ToString()
                });
            ViewBag.Types = Enum.GetValues<TransactionType>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = t.ToString()
                });
            ViewBag.Statuses = Enum.GetValues<TransactionStatus>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),
                    Text = s.ToString()
                });
        }
    }
}
