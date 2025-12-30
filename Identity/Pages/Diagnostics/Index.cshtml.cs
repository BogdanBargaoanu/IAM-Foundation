using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Diagnostics;

[SecurityHeaders]
[Authorize]
public class Index : PageModel
{
    private readonly IConfiguration _configuration;

    public Index(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public ViewModel View { get; set; } = default!;

    public async Task<IActionResult> OnGet()
    {
        var allowRemote = _configuration.GetValue<bool>("ALLOW_REMOTE_DIAGNOSTICS", false);

        if (!allowRemote && HttpContext.Connection.IsRemote())
        {
            return NotFound();
        }

        View = new ViewModel(await HttpContext.AuthenticateAsync());

        return Page();
    }
}
