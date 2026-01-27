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
        private const string BaseUrlV1 = "/api/v1/transactions";
        private const string BaseUrlV2 = "/api/v2/transactions";

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
                HttpRequestType.Get,
                $"{BaseUrlV1}/amounts",
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
            ViewBag.CurlCommand = _curlBuilder.BuildCommand(
                HttpRequestType.Get,
                $"{BaseUrlV2}/accounts/{accountId}");

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
                HttpRequestType.Get,
                $"{BaseUrlV1}/amounts",
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
            ViewBag.CurlCommand = _curlBuilder.BuildCommand(
                HttpRequestType.Get,
                $"{BaseUrlV1}/amounts",
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
                HttpRequestType.Get,
                $"{BaseUrlV1}/amounts",
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

        [HttpPost]
        public async Task<IActionResult> GetTransactionById(Guid transactionId)
        {
            ViewBag.CurlCommand = _curlBuilder.BuildCommand(
                HttpRequestType.Get,
                $"{BaseUrlV2}/{transactionId}");

            try
            {
                var transaction = await _apiClient.GetByIdAsync(transactionId);
                ViewBag.Result = transaction != null
                    ? @$"Transaction Found: ID={transaction.Id}, AccountID={transaction.AccountId}, Amount={transaction.Amount:C} {transaction.Currency}, Type={transaction.Type},
                        Status={transaction.Status}, Merchant={transaction.MerchantName}, Reference={transaction.Reference}, Timestamp={transaction.Timestamp}"
                    : "Transaction not found.";
                ViewBag.ResultType = "success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transaction details");
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
            TransactionStatus? status,
            int page = 1,
            int pageSize = 10)
        {
            if (TempData.TryGetValue("CurlCommandBefore", out var curl))
            {
                ViewBag.CurlCommand = curl?.ToString();
            }

            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accountId)) queryParams["accountId"] = accountId;
            if (!string.IsNullOrEmpty(merchantName)) queryParams["merchantName"] = merchantName;
            if (!string.IsNullOrEmpty(reference)) queryParams["reference"] = reference;
            if (currency.HasValue) queryParams["currency"] = ((int)currency).ToString();
            if (type.HasValue) queryParams["type"] = ((int)type).ToString();
            if (status.HasValue) queryParams["status"] = ((int)status).ToString();
            queryParams["page"] = page.ToString();
            queryParams["pageSize"] = pageSize.ToString();

            if (ViewBag.CurlCommand is null)
            {
                ViewBag.CurlCommand = _curlBuilder.BuildCommand(
                    HttpRequestType.Get,
                    BaseUrlV1,
                    queryParams);
            }

            PopulateDropdowns();

            try
            {
                var transactions = await _apiClient.GetTransactionsAsync(
                    accountId, merchantName, reference, currency, type, status);

                return View(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transactions");
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<Transaction>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(Transaction transaction)
        {
            TempData["CurlCommandBefore"] = _curlBuilder.BuildCommand(
                HttpRequestType.Post,
                BaseUrlV2,
                jsonBody: transaction);

            PopulateDropdowns();

            try
            {
                var created = await _apiClient.CreateTransactionAsync(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction");
                ViewBag.Result = $"Error: {ex.Message}";
            }
            return RedirectToRefererOrDefault();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTransaction(Guid id, Transaction transaction)
        {
            TempData["CurlCommandBefore"] = _curlBuilder.BuildCommand(
                HttpRequestType.Put,
                $"{BaseUrlV2}/{id}",
                jsonBody: transaction);

            PopulateDropdowns();

            try
            {
                var updated = await _apiClient.UpdateTransactionAsync(id, transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transaction with ID: {TransactionId}", id);
                ViewBag.Result = $"Error: {ex.Message}";
            }
            return RedirectToRefererOrDefault();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTransaction(Guid id)
        {
            TempData["CurlCommandBefore"] = _curlBuilder.BuildCommand(
                HttpRequestType.Delete,
                $"{BaseUrlV2}/{id}");

            PopulateDropdowns();

            try
            {
                await _apiClient.DeleteTransactionAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transaction with ID: {TransactionId}", id);
                ViewBag.Result = $"Error: {ex.Message}";
            }
            return RedirectToRefererOrDefault();
        }

        private IActionResult RedirectToRefererOrDefault()
        {
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrWhiteSpace(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction(nameof(Transactions));
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
