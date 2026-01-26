using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsApi.Mock
{
    public static class MockData
    {
        public static IReadOnlyList<Transaction> Transactions => new[]
        {
            new Transaction(
                Guid.Parse("1DB8474B-8AB4-47F3-8199-A0623D7F9A69"),
                "acc-001",
                DateTimeOffset.UtcNow.AddDays(-2),
                520.15m,
                TransactionCurrency.USD,
                TransactionType.Debit,
                TransactionStatus.Initiated,
                "CoffeeUnlimited LTD",
                "Coffee and pastry",
                "POS-10001"),

            new Transaction(
                Guid.Parse("22993044-A027-4164-B89E-9BDE14E5260A"),
                "acc-001",
                DateTimeOffset.UtcNow.AddDays(-2),
                102.25m,
                TransactionCurrency.RON,
                TransactionType.Debit,
                TransactionStatus.Successful,
                "CoffeeUnlimited LTD",
                "Coffee machine repair",
                "POS-10001"),

            new Transaction(
                Guid.Parse("0F0C2BC1-7C1A-4B6E-8C7B-1E9C5D4B7A11"),
                "acc-001",
                DateTimeOffset.UtcNow.AddDays(-1).AddHours(-2),
                1250.00m,
                TransactionCurrency.USD,
                TransactionType.Credit,
                TransactionStatus.Successful,
                "Contoso Payroll",
                "Salary payment",
                "POS-778812"),

            new Transaction(
                Guid.Parse("6C2B8F6D-8A5C-4F12-9F39-7C2D0A6A2E8C"),
                "acc-002",
                DateTimeOffset.UtcNow.AddDays(-6).AddHours(-4),
                89.99m,
                TransactionCurrency.EUR,
                TransactionType.Debit,
                TransactionStatus.Successful,
                "Electronics For All",
                "Online purchase",
                "POS-55021"),

            new Transaction(
                Guid.Parse("A7B4C3D2-1E5F-4A9B-8D76-2F3C4B5A6D7E"),
                "acc-002",
                DateTimeOffset.UtcNow.AddDays(-5).AddMinutes(-30),
                24.10m,
                TransactionCurrency.RON,
                TransactionType.Debit,
                TransactionStatus.Successful,
                "CoffeeUnlimited LTD",
                "Espresso & water",
                "POS-10002"),

            new Transaction(
                Guid.Parse("B9E1F2A3-4C5D-6E7F-8A9B-0C1D2E3F4A5B"),
                "acc-003",
                DateTimeOffset.UtcNow.AddDays(-3).AddHours(-7),
                199.95m,
                TransactionCurrency.GBP,
                TransactionType.Debit,
                TransactionStatus.Failed,
                "Northwind Grocers",
                "Grocery basket (card declined)",
                "POS-20001"),

            new Transaction(
                Guid.Parse("D4C3B2A1-0F9E-8D7C-6B5A-4E3D2C1B0A9F"),
                "acc-003",
                DateTimeOffset.UtcNow.AddDays(-2).AddHours(-10),
                499.00m,
                TransactionCurrency.USD,
                TransactionType.Debit,
                TransactionStatus.Successful,
                "Tailspin Toys",
                "Gift purchase",
                "POS-30001"),

            new Transaction(
                Guid.Parse("E2F3A4B5-C6D7-4890-AB12-CD34EF56AB78"),
                "acc-001",
                DateTimeOffset.UtcNow.AddDays(-1).AddHours(-12),
                15.75m,
                TransactionCurrency.USD,
                TransactionType.Debit,
                TransactionStatus.Successful,
                "CoffeeUnlimited LTD",
                "Cappuccino",
                "POS-10001")
        };
    }
}
