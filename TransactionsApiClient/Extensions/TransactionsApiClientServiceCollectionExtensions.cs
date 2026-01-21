using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using TransactionsApiClient.Services.ApiClient;
using TransactionsApiClient.Services.Auth.Handlers;
using TransactionsApiClient.Services.Auth.Providers;

namespace TransactionsApiClient.Extensions;

public static class TransactionsApiClientServiceCollectionExtensions
{
    public static IServiceCollection AddTransactionsApiClient(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddTransient<AccessTokenDelegatingHandler>();

        var baseUrl = configuration["TransactionsApi:BaseUrl"] ?? "https://localhost:7001";

        services.AddHttpClient<ITransactionsApiClient, TransactionsApiClient.Services.ApiClient.TransactionsApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddHttpMessageHandler<AccessTokenDelegatingHandler>();

        return services;
    }

    public static IServiceCollection AddTransactionsApiClientUsingHttpContext(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IAccessTokenProvider, HttpContextAccessTokenProvider>();

        return services.AddTransactionsApiClient(configuration, environment);
    }
}