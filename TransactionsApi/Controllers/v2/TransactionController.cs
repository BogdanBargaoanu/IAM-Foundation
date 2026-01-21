using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionsApi.Services;
using TransactionsLibrary.Constants;

namespace TransactionsApi.Controllers.v2
{
    [Authorize("ApiScope")]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;
        public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet("accounts/{accountId}")]
        public ActionResult<IReadOnlyDictionary<TransactionCurrency, decimal>> GetAccountTotal(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                _logger.LogInformation("Attempted to get account totals with missing AccountId.");
                return BadRequest("AccountId cannot be missing.");
            }

            var totals = _transactionService.GetAccountTotal(accountId);

            return Ok(totals);
        }
    }
}
