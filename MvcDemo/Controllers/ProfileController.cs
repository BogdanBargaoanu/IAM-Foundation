using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MvcDemo.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var userName = User.Identity?.Name ?? User.FindFirstValue("name");
            var subjectId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            var claims = User.Claims
                .Select(c => new ClaimViewModel(c.Type, c.Value))
                .OrderBy(c => c.Type)
                .ToList();

            // Retrieve tokens saved in the auth cookie
            var token = await HttpContext.GetUserAccessTokenAsync();
            var accessToken = token.Token.AccessToken;
            var idToken = token.Token.IdentityToken;
            var refreshToken = token.Token.RefreshToken;

            var vm = new ProfileViewModel(userName, subjectId, claims)
            {
                AccessToken = accessToken,
                IdToken = idToken,
                RefreshToken = refreshToken
            };
            return View(vm);
        }
        [HttpGet]
        public IActionResult Logout()
        {
            var redirectUri = Url.Action("Index", "Home");
            var authProps = new AuthenticationProperties { RedirectUri = redirectUri };

            // Triggers OIDC sign-out (front-channel) and clears local cookie
            return SignOut(authProps, OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public record ClaimViewModel(string Type, string Value);

    public record ProfileViewModel(string? UserName, string? SubjectId, IReadOnlyList<ClaimViewModel> Claims)
    {
        public string? AccessToken { get; init; }
        public string? IdToken { get; init; }
        public string? RefreshToken { get; init; }
    }
}