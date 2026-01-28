using TransactionsLibrary.Constants;

namespace TransactionsLibrary.Models
{
    public sealed class Transaction
    {
        public Transaction()
        {
        }

        public Transaction(
            Guid id,
            string accountId,
            DateTimeOffset timestamp,
            decimal amount,
            TransactionCurrency currency,
            TransactionType type,
            TransactionStatus status,
            string? merchantName,
            string? description,
            string? reference)
        {
            Id = id;
            AccountId = accountId;
            Timestamp = timestamp;
            Amount = amount;
            Currency = currency;
            Type = type;
            Status = status;
            MerchantName = merchantName;
            Description = description;
            Reference = reference;
        }
        public Guid Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; }
        public decimal Amount { get; set; }
        public TransactionCurrency Currency { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public string? MerchantName { get; set; }
        public string? Description { get; set; }
        public string? Reference { get; set; }
    }
}
