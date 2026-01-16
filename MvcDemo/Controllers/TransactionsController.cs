using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcDemo.Services.ApiClient;
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
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            _apiClient.InjectAccessToken(accessToken ?? string.Empty);
            bool isHealthy = false;
            try
            {
                isHealthy = await _apiClient.CheckHealthy();
            }
            catch (Exception)
            {
                isHealthy = false;
            }
            ViewBag.ServiceHealth = isHealthy;

            try
            {
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
