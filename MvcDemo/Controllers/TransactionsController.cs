using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
