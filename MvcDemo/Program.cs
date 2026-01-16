using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        builder.Configuration.Bind("Authentication:Schemes:OpenIdConnect", options);

        options.ResponseType = "code";
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

        if (builder.Environment.IsDevelopment())
        {
            options.RequireHttpsMetadata = false;

            options.BackchannelHttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = static (_, _, _, _) => true
            };
        }

        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();

                // Log the code challenge
                logger.LogInformation("Code Challenge: {CodeChallenge}",
                    context.ProtocolMessage.Parameters.TryGetValue("code_challenge", out var challenge) ? challenge : "N/A");
                logger.LogInformation("Code Challenge Method: {Method}",
                    context.ProtocolMessage.Parameters.TryGetValue("code_challenge_method", out var method) ? method : "N/A");

                logger.LogInformation("Client ID: {ClientId}", context.ProtocolMessage.ClientId);
                logger.LogInformation("Redirect URI: {RedirectUri}", context.ProtocolMessage.RedirectUri);
                logger.LogInformation("Scopes: {Scopes}", context.ProtocolMessage.Scope);

                return Task.CompletedTask;
            },

            OnAuthorizationCodeReceived = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("Authorization Code Received: {Code}", context.ProtocolMessage.Code ?? "N/A");

                return Task.CompletedTask;
            },

            OnTokenResponseReceived = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("Token Response Received: {Token}, type: {TokenType}",
                    context.TokenEndpointResponse.AccessToken ?? "N/A",
                    context.TokenEndpointResponse.TokenType);

                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "profile",
    pattern: "profile",
    defaults: new { controller = "Profile", action = "Index" });

app.Run();
