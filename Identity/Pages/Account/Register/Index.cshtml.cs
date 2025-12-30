using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Account.Register;

[SecurityHeaders]
[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;

    public IndexModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interaction)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _interaction = interaction;
    }

    [BindProperty]
    public RegisterModel Input { get; set; } = default!;

    public void OnGet(string? returnUrl)
    {
        Input = new RegisterModel
        {
            ReturnUrl = returnUrl,
        };
    }

    public async Task<IActionResult> OnPost()
    {
        // cancel behaves like login cancel
        if (Input.Button != "register")
        {
            var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);
            if (context != null)
            {
                ArgumentNullException.ThrowIfNull(Input.ReturnUrl, nameof(Input.ReturnUrl));
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                if (context.IsNativeClient())
                {
                    return this.LoadingPage(Input.ReturnUrl);
                }

                return Redirect(Input.ReturnUrl ?? "~/");
            }

            return Redirect("~/");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingByName = await _userManager.FindByNameAsync(Input.Username!);
        if (existingByName != null)
        {
            ModelState.AddModelError(string.Empty, RegisterOptions.InvalidUsernameErrorMessage);
            return Page();
        }

        var existingByEmail = await _userManager.FindByEmailAsync(Input.Email!);
        if (existingByEmail != null)
        {
            ModelState.AddModelError(string.Empty, RegisterOptions.InvalidEmailErrorMessage);
            return Page();
        }

        if (Input.Password!.Length < 8 ||
            !Input.Password.Any(char.IsUpper) ||
            !Input.Password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            ModelState.AddModelError(string.Empty, RegisterOptions.InvalidPasswordErrorMessage);
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Username,
            Email = Input.Email,
            EmailConfirmed = false,
        };

        var createResult = await _userManager.CreateAsync(user, Input.Password!);
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);

        var contextAfterRegister = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);
        if (contextAfterRegister != null && contextAfterRegister.IsNativeClient())
        {
            ArgumentNullException.ThrowIfNull(Input.ReturnUrl, nameof(Input.ReturnUrl));
            return this.LoadingPage(Input.ReturnUrl);
        }

        if (Url.IsLocalUrl(Input.ReturnUrl))
        {
            return Redirect(Input.ReturnUrl!);
        }

        return Redirect("~/");
    }
}
