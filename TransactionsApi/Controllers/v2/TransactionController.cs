using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionsApi.Constants;
using TransactionsApi.Services;

namespace TransactionsApi.Controllers.v2
{
    [Authorize("ApiScope")]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;
        public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet("account/{accountId}")]
        public ActionResult<IReadOnlyDictionary<TransactionCurrency, decimal>> GetAccountTotal(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                _logger.LogInformation("Attempted to get account totals with missing AccountId.");
                return BadRequest("AccountId cannot be missing.");
            }

            var totals = Enum.GetValues<TransactionCurrency>()
                .ToDictionary(
                    currency => currency,
                    currency => _transactionService.GetAccountTotal(accountId, currency));

            return Ok(totals);
        }
    }
}
