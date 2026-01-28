using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionsApi.Services;
using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsApi.Controllers.v2
{
    [Authorize(Policies.ApiScope)]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IAuthorizationService _accountOwnerService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService transactionService, IAuthorizationService accountOwnerService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _accountOwnerService = accountOwnerService;
            _logger = logger;
        }

        [HttpGet("accounts/{accountId}")]
        public async Task<ActionResult<IReadOnlyDictionary<TransactionCurrency, decimal>>> GetAccountTotal(
            string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                _logger.LogInformation("Attempted to get account totals with missing AccountId.");
                return BadRequest("AccountId cannot be missing.");
            }

            var totals = await _transactionService.GetAccountTotalAsync(accountId);

            return Ok(totals);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Transaction>> GetTransactionById(Guid id)
        {
            var transaction = await _transactionService.GetByIdAsync(id);
            if (transaction == null)
            {
                _logger.LogInformation("Transaction with ID {TransactionId} not found.", id);
                return NotFound();
            }

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction([FromBody] Transaction transaction)
        {
            try
            {
                var authResult = await _accountOwnerService.AuthorizeAsync(User, transaction, Policies.AccountOwner);
                if (!authResult.Succeeded) return Forbid();

                var created = await _transactionService.CreateTransactionAsync(transaction);
                return CreatedAtAction(
                    nameof(GetTransactionById),
                    new { id = created.Id, version = "2.0" },
                    created);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Transaction>> UpdateTransaction(Guid id, [FromBody] Transaction transaction)
        {
            try
            {
                var existing = await _transactionService.GetByIdAsync(id);

                var authResult = await _accountOwnerService.AuthorizeAsync(User, existing, Policies.AccountOwner);
                if (!authResult.Succeeded) return Forbid();

                var updated = await _transactionService.UpdateTransactionAsync(id, transaction);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteTransaction(Guid id)
        {
            var existing = await _transactionService.GetByIdAsync(id);

            var authResult = await _accountOwnerService.AuthorizeAsync(User, existing, Policies.AccountOwner);
            if (!authResult.Succeeded) return Forbid();

            var deleted = await _transactionService.DeleteTransactionAsync(id);
            if (!deleted)
            {
                _logger.LogInformation("Attempted to delete non-existent transaction with ID {TransactionId}", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}
