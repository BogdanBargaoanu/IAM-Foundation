using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.ExtensionGrants
{
    public class KioskAuthenticationGrantValidator : IExtensionGrantValidator
    {
        private readonly ILogger<KioskAuthenticationGrantValidator> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public KioskAuthenticationGrantValidator(ILogger<KioskAuthenticationGrantValidator> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public string GrantType => "kiosk_auth";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            // default response to invalid
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);

            var employeeId = context.Request.Raw.Get("employee_id");
            var pin = context.Request.Raw.Get("pin");

            if (string.IsNullOrEmpty(employeeId) || string.IsNullOrEmpty(pin))
            {
                _logger.LogWarning("Kiosk authentication failed: missing employee_id or pin");
                context.Result = new GrantValidationResult(
                   TokenRequestErrors.InvalidRequest,
                   "employee_id and pin are required"
                );
                return;
            }

            var user = await ValidateEmployeeCredentialsAsync(employeeId, pin);

            if (user is null)
            {
                _logger.LogWarning("Kiosk authentication failed: invalid credentials for employee_id {EmployeeId}", employeeId);
                context.Result = new GrantValidationResult(
                   TokenRequestErrors.InvalidGrant,
                   "Invalid employee credentials"
                );
                return;
            }

            context.Result = new GrantValidationResult(
                subject: user.Id,
                authenticationMethod: GrantType);
        }

        private async Task<ApplicationUser?> ValidateEmployeeCredentialsAsync(string employeeId, string pin)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.EmployeeId == employeeId);
            if (user is null || string.IsNullOrEmpty(user.PinHash))
            {
                return null;
            }

            var expectedPinHash = pin.Sha256();
            return string.Equals(user.PinHash, expectedPinHash, StringComparison.Ordinal) ? user : null;
        }
    }
}
