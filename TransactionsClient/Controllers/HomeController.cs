using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TransactionsClient.Models;
using TransactionsClient.Services.ApiClient;

namespace TransactionsClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITransactionsApiClient _apiClient;
        public HomeController(ITransactionsApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            bool isHealthy = false;
            try
            {
                isHealthy = await _apiClient.CheckHealthy();
            }
            catch (Exception)
            {
                // If the health check fails, isHealthy remains false
                isHealthy = false;
            }
            ViewBag.ServiceHealth = isHealthy;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
