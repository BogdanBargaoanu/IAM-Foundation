namespace TransactionsClient.Services.TokenService
{
    public interface ITokenService
    {
        Task<string> GetAccesTokenAsync();
    }
}
