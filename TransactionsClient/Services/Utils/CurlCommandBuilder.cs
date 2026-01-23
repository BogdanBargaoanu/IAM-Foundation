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
        public string BuildCommand(string endpoint, Dictionary<string, string>? queryParams = null)
        {
            var query = string.Empty;
            if (queryParams?.Any() == true)
            {
                query = "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            }

            return $"curl -X GET \"{GetBaseUrl()}{endpoint}{query}\" \\\n  -H \"Authorization: Bearer {{TOKEN}}\"";
        }
    }
}
