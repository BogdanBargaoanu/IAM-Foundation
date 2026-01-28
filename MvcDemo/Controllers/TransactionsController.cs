using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TransactionsApiClient.Services.ApiClient;
using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace MvcDemo.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ITransactionsApiClient _apiClient;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(
            ITransactionsApiClient apiClient,
            ILogger<TransactionsController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            int page = Pagination.DefaultPageIndex,
            int pageSize = Pagination.DefaultPageSize)
        {
            PopulateDropdowns();

            bool isHealthy = false;
            try
            {
                isHealthy = await _apiClient.CheckHealthyAsync();
            }
            catch (Exception)
            {
                isHealthy = false;
            }
            ViewBag.ServiceHealth = isHealthy;

            try
            {
                _logger.LogInformation("Fetching transactions");

                var count = await _apiClient.GetCountAsync();
                var transactions = await _apiClient.GetTransactionsAsync(page: page, pageSize: pageSize);

                var paginatedResult = new PaginatedList<Transaction>(transactions.ToList(), count, page, pageSize);

                return View(paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transactions");
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new PaginatedList<Transaction>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTransaction(Guid id, Transaction transaction)
        {
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
            return RedirectToAction(nameof(Index));
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
