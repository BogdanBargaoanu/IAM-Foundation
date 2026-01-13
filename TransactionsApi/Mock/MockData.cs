using TransactionsApi.Constants;
using TransactionsApi.Models;

namespace TransactionsApi.Mock
{
    public static class MockData
    {
        public static IReadOnlyList<Transaction> Transactions => new[]
        {
            new Transaction(
                Id: Guid.Parse("1DB8474B-8AB4-47F3-8199-A0623D7F9A69"),
                AccountId: "acc-001",
                Timestamp: DateTimeOffset.UtcNow.AddDays(-2),
                Amount: 520.15m,
                Currency: TransactionCurrency.USD,
                Type: TransactionType.Debit,
                Status: TransactionStatus.Initiated,
                MerchantName: "CoffeeUnlimited LTD",
                Description: "Coffee and pastry",
                Reference: "POS-10001"),

            new Transaction(
                Id: Guid.Parse("22993044-A027-4164-B89E-9BDE14E5260A"),
                AccountId: "acc-001",
                Timestamp: DateTimeOffset.UtcNow.AddDays(-2),
                Amount: 102.25m,
                Currency: TransactionCurrency.RON,
                Type: TransactionType.Debit,
                Status: TransactionStatus.Successful,
                MerchantName: "CoffeeUnlimited LTD",
                Description: "Coffee machine repair",
                Reference: "POS-10001"),

            new Transaction(
                Id: Guid.Parse("0F0C2BC1-7C1A-4B6E-8C7B-1E9C5D4B7A11"),
                AccountId: "acc-001",
                Timestamp: DateTimeOffset.UtcNow.AddDays(-1).AddHours(-2),
                Amount: 1250.00m,
                Currency: TransactionCurrency.USD,
                Type: TransactionType.Credit,
                Status: TransactionStatus.Successful,
                MerchantName: "Contoso Payroll",
                Description: "Salary payment",
                Reference: "POS-778812"),

            new Transaction(
                Id: Guid.Parse("6C2B8F6D-8A5C-4F12-9F39-7C2D0A6A2E8C"),
                AccountId: "acc-002",
                Timestamp: DateTimeOffset.UtcNow.AddDays(-6).AddHours(-4),
                Amount: 89.99m,
                Currency: TransactionCurrency.EUR,
                Type: TransactionType.Debit,
                Status: TransactionStatus.Successful,
                MerchantName: "Electronics For All",
                Description: "Online purchase",
                Reference: "POS-55021"),

            new Transaction(
                Id: Guid.Parse("A7B4C3D2-1E5F-4A9B-8D76-2F3C4B5A6D7E"),
                AccountId: "acc-002",
                Timestamp: DateTimeOffset.UtcNow.AddDays(-5).AddMinutes(-30),
                Amount: 24.10m,
                Currency: TransactionCurrency.RON,
                Type: TransactionType.Debit,
                Status: TransactionStatus.Successful,
                MerchantName: "CoffeeUnlimited LTD",
                Description: "Espresso & water",
                Reference: "POS-10002"),

            new Transaction(
                Id: Guid.Parse("B9E1F2A3-4C5D-6E7F-8A9B-0C1D2E3F4A5B"),
                AccountId: "acc-003",
                Timestamp: DateTimeOffset.UtcNow.AddDays(-3).AddHours(-7),
                Amount: 199.95m,
                Currency: TransactionCurrency.GBP,
                Type: TransactionType.Debit,
                Status: TransactionStatus.Failed,
                MerchantName: "Northwind Grocers",
                Description: "Grocery basket (card declined)",
                Reference: "POS-20001"),

            new Transaction(
                Id: Guid.Parse("D4C3B2A1-0F9E-8D7C-6B5A-4E3D2C1B0A9F"),
                AccountId: "acc-003",
                Timestamp: DateTimeOffset.UtcNow.AddDays(-2).AddHours(-10),
                Amount: 499.00m,
                Currency: TransactionCurrency.USD,
                Type: TransactionType.Debit,
                Status: TransactionStatus.Successful,
                MerchantName: "Tailspin Toys",
                Description: "Gift purchase",
                Reference: "POS-30001"),

            new Transaction(
                Id: Guid.Parse("E2F3A4B5-C6D7-4890-AB12-CD34EF56AB78"),
                AccountId: "acc-001",
                Timestamp: DateTimeOffset.UtcNow.AddDays(-1).AddHours(-12),
                Amount: 15.75m,
                Currency: TransactionCurrency.USD,
                Type: TransactionType.Debit,
                Status: TransactionStatus.Successful,
                MerchantName: "CoffeeUnlimited LTD",
                Description: "Cappuccino",
                Reference: "POS-10001")
        };
    }
}
