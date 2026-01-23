using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionsApiClient.Services.ApiClient;
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

        public async Task<IActionResult> Index()
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
                var transactions = await _apiClient.GetTransactionsAsync();
                return View(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transactions");
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<Transaction>());
            }
        }
    }
}
