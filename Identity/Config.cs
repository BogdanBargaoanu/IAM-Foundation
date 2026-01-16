using Duende.IdentityServer.Models;

namespace Identity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new[]
        {
            new ApiScope("kiosk_api"),
            new ApiScope("transactions_api")
        };

    public static IEnumerable<Client> Clients =>
        new[]
        {
            // mvc client using code flow + pkce
            new Client
            {
                ClientId = "mvc.frontend.demo",
                ClientName = "Demo MVC Frontend Client",
                ClientSecrets = { new Secret("D7EBB0B8-F964-480A-B992-7FACDF747EFE".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "offline_access", "transactions_api" },
                RedirectUris = { "https://localhost:7151/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:7151/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:7151/signout-callback-oidc" },
            },

            // kiosk client using custom grant type
            new Client
            {
                ClientId = "kiosk.client",
                ClientName = "Kiosk Client",
                AllowedGrantTypes = { "kiosk_auth" },
                ClientSecrets = { new Secret("8B003741-8ACF-483B-AC20-F904CF3AF860".Sha256()) },
                AllowedScopes = { "kiosk_api" },
                RequirePkce = false
            },

            // finance client using client credentials flow
            new Client
            {
                ClientId = "transactions.client",
                ClientName = "Transactions Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("4BF97A40-EE03-4A6F-B0E2-6544712276A3".Sha256()) },

                AllowedScopes = { "transactions_api" }
            },
        };
}