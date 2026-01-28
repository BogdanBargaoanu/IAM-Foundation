using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TransactionsLibrary.Models;

namespace TransactionsApi.Services.Auth
{
    public class TransactionOwnerAuthorizationHandler : AuthorizationHandler<AccountOwnerRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccountOwnerRequirement requirement)
        {
            if (context.Resource is not Transaction transaction)
            {
                return Task.CompletedTask;
            }

            var clientId = context.User.FindFirstValue("client_id");

            if (clientId == "transactions.client")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var userId = context.User.FindFirstValue("sub") ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null && transaction.AccountId == userId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
