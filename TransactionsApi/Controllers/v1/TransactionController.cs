using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TransactionsApi.Services;
using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsApi.Controllers.v1
{
    [Authorize(Policies.ApiScope)]
    [ApiController]
    [ApiVersion("1.0")]
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

        [HttpGet("amounts")]
        public async Task<ActionResult<decimal>> GetBalanceForCurrency(
            [Required][FromQuery] TransactionCurrency currency,
            [FromQuery] SearchCriteria searchBy = SearchCriteria.None,
            [FromQuery] string? searchValue = null)
        {
            try
            {
                var balance = await _transactionService.GetBalanceForCurrencyAsync(currency, searchBy, searchValue);
                return Ok(balance);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid search parameter provided.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCount(
            [FromQuery] string? accountId = null,
            [FromQuery] string? merchantName = null,
            [FromQuery] string? reference = null,
            [FromQuery] TransactionCurrency? currency = null,
            [FromQuery] TransactionType? type = null,
            [FromQuery] TransactionStatus? status = null)
        {
            var count = await _transactionService.GetCountAsync(accountId, merchantName, reference, currency, type, status);

            return Ok(count);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Transaction>>> GetTransactions(
            [FromQuery] string? accountId = null,
            [FromQuery] string? merchantName = null,
            [FromQuery] string? reference = null,
            [FromQuery] TransactionCurrency? currency = null,
            [FromQuery] TransactionType? type = null,
            [FromQuery] TransactionStatus? status = null,
            [Required][FromQuery] int page = 1,
            [Required][FromQuery] int pageSize = 10)
        {
            var transactions = await _transactionService.GetTransactionsAsync(accountId, merchantName, reference,
                currency, type, status, page, pageSize);

            return Ok(transactions);
        }
    }
}
