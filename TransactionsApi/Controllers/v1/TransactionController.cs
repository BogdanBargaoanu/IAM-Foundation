using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionsApi.Constants;
using TransactionsApi.Services;

namespace TransactionsApi.Controllers.v1
{
    [Authorize("ApiScope")]
    [ApiController]
    [ApiVersion("1.0")]
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
        public ActionResult<decimal> GetAccountTotal(string accountId, [FromQuery] TransactionCurrency currency)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                return BadRequest("AccountId cannot be missing.");
            }

            var total = _transactionService.GetAccountTotal(accountId, currency);
            return Ok(total);
        }

        [HttpGet("merchant/{merchantName}")]
        public ActionResult<decimal> GetMerchantTotal(string merchantName, [FromQuery] TransactionCurrency currency)
        {
            if (string.IsNullOrWhiteSpace(merchantName))
            {
                return BadRequest("MerchantName cannot be missing.");
            }

            var total = _transactionService.GetMerchantTotal(merchantName, currency);
            return Ok(total);
        }

        [HttpGet("currency/{currency}")]
        public ActionResult<decimal> GetCurrencyTotal(TransactionCurrency currency)
        {
            var total = _transactionService.GetCurrencyTotal(currency);
            return Ok(total);
        }

        [HttpGet("reference/{reference}")]
        public ActionResult<decimal> GetReferenceTotal(string reference, [FromQuery] TransactionCurrency currency)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return BadRequest("Reference cannot be missing.");
            }
            var total = _transactionService.GetReferenceTotal(reference, currency);
            return Ok(total);
        }

        [HttpGet("transactions")]
        public ActionResult<IReadOnlyList<Models.Transaction>> GetTransactions(
            [FromQuery] string? accountId = null,
            [FromQuery] string? merchantName = null,
            [FromQuery] string? reference = null,
            [FromQuery] TransactionCurrency? currency = null,
            [FromQuery] TransactionType? type = null,
            [FromQuery] TransactionStatus? status = null)
        {
            var transactions = _transactionService.GetTransactions(
                accountId,
                merchantName,
                reference,
                currency,
                type,
                status);
            return Ok(transactions);
        }
    }
}
