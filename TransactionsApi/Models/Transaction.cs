using TransactionsApi.Constants;

namespace TransactionsApi.Models
{
    public sealed record Transaction(
        Guid Id,
        string AccountId,
        DateTimeOffset Timestamp,
        decimal Amount,
        TransactionCurrency Currency,
        TransactionType Type,
        TransactionStatus Status,
        string? MerchantName,
        string? Description,
        string? Reference);
}
