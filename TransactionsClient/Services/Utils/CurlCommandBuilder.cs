using System.Text;
using System.Text.Json;

namespace TransactionsClient.Services.Utils
{
    public class CurlCommandBuilder
    {
        private readonly IConfiguration _configuration;

        public CurlCommandBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetBaseUrl()
        {
            return _configuration["TransactionsApi:BaseUrl"] ?? "https://localhost:7001";
        }

        public string BuildCommand(
            HttpRequestType method,
            string endpoint,
            Dictionary<string, string>? queryParams = null,
            object? jsonBody = null)
        {
            var url = BuildUrl(endpoint, queryParams);

            var sb = new StringBuilder();
            sb.Append($"curl -X {ToVerb(method)} \\\"{url}\\\" \\\n");
            sb.Append("  -H \\\"Authorization: Bearer {TOKEN}\\\"");

            if (jsonBody is not null)
            {
                var payload = JsonSerializer.Serialize(jsonBody);
                sb.Append(" \\\n");
                sb.Append("  -H \\\"Content-Type: application/json\\\" \\\n");
                sb.Append($"  -d \\\"{EscapeForCurlDoubleQuotes(payload)}\\\"");
            }

            return sb.ToString();
        }

        private string BuildUrl(string endpoint, Dictionary<string, string>? queryParams)
        {
            var baseUrl = GetBaseUrl().TrimEnd('/');
            var path = endpoint.StartsWith('/') ? endpoint : "/" + endpoint;

            if (queryParams?.Any() != true)
            {
                return $"{baseUrl}{path}";
            }

            var query = string.Join("&",
                queryParams.Select(kvp =>
                    $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            return $"{baseUrl}{path}?{query}";
        }

        private static string ToVerb(HttpRequestType method) => method switch
        {
            HttpRequestType.Get => "GET",
            HttpRequestType.Post => "POST",
            HttpRequestType.Put => "PUT",
            HttpRequestType.Patch => "PATCH",
            HttpRequestType.Delete => "DELETE",
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, "Unsupported HTTP verb")
        };

        private static string EscapeForCurlDoubleQuotes(string s)
            => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
