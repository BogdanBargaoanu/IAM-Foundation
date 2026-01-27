using TransactionsLibrary.Constants;
using TransactionsLibrary.Models;

namespace TransactionsApi.Mock
{
    public static class MockData
    {
        public static IReadOnlyList<Transaction> Transactions => new[]
        {
            new Transaction(Guid.NewGuid(), "acc-001", DateTimeOffset.UtcNow.AddDays(-2), 520.15m, TransactionCurrency.USD, TransactionType.Debit, TransactionStatus.Initiated, "CoffeeUnlimited LTD", "Coffee and pastry", "POS-10001"),
            new Transaction(Guid.NewGuid(), "acc-001", DateTimeOffset.UtcNow.AddDays(-2), 102.25m, TransactionCurrency.RON, TransactionType.Debit, TransactionStatus.Successful, "CoffeeUnlimited LTD", "Coffee machine repair", "POS-10001"),
            new Transaction(Guid.NewGuid(), "acc-001", DateTimeOffset.UtcNow.AddDays(-1).AddHours(-2), 1250.00m, TransactionCurrency.USD, TransactionType.Credit, TransactionStatus.Successful, "Contoso Payroll", "Salary payment", "POS-778812"),
            new Transaction(Guid.NewGuid(), "acc-002", DateTimeOffset.UtcNow.AddDays(-6).AddHours(-4), 89.99m, TransactionCurrency.EUR, TransactionType.Debit, TransactionStatus.Successful, "Electronics For All", "Online purchase", "POS-55021"),
            new Transaction(Guid.NewGuid(), "acc-002", DateTimeOffset.UtcNow.AddDays(-5).AddMinutes(-30), 24.10m, TransactionCurrency.RON, TransactionType.Debit, TransactionStatus.Successful, "CoffeeUnlimited LTD", "Espresso & water", "POS-10002"),
            new Transaction(Guid.NewGuid(), "acc-003", DateTimeOffset.UtcNow.AddDays(-3).AddHours(-7), 199.95m, TransactionCurrency.GBP, TransactionType.Debit, TransactionStatus.Failed, "Northwind Grocers", "Grocery basket (card declined)", "POS-20001"),
            new Transaction(Guid.NewGuid(), "acc-003", DateTimeOffset.UtcNow.AddDays(-2).AddHours(-10), 499.00m, TransactionCurrency.USD, TransactionType.Debit, TransactionStatus.Successful, "Tailspin Toys", "Gift purchase", "POS-30001"),
            new Transaction(Guid.NewGuid(), "acc-001", DateTimeOffset.UtcNow.AddDays(-1).AddHours(-12), 15.75m, TransactionCurrency.USD, TransactionType.Debit, TransactionStatus.Successful, "CoffeeUnlimited LTD", "Cappuccino", "POS-10001"),
            new Transaction(Guid.NewGuid(), "acc-001", DateTimeOffset.UtcNow.AddDays(-1), 120.50m, TransactionCurrency.EUR, TransactionType.Credit, TransactionStatus.Successful, "CoffeeUnlimited LTD", "Coffee and pastry", "POS-10001"),
            new Transaction(Guid.NewGuid(), "acc-002", DateTimeOffset.UtcNow.AddDays(-2), 75.00m, TransactionCurrency.RON, TransactionType.Debit, TransactionStatus.Failed, "Northwind Grocers", "Groceries", "POS-20001"),
            new Transaction(Guid.NewGuid(), "acc-003", DateTimeOffset.UtcNow.AddDays(-3), 200.00m, TransactionCurrency.USD, TransactionType.Credit, TransactionStatus.Successful, "Contoso Payroll", "Salary payment", "POS-30001"),
            new Transaction(Guid.NewGuid(), "acc-004", DateTimeOffset.UtcNow.AddDays(-4), 15.75m, TransactionCurrency.EUR, TransactionType.Credit, TransactionStatus.Successful, "CoffeeUnlimited LTD", "Cappuccino", "POS-10002"),
            new Transaction(Guid.NewGuid(), "acc-005", DateTimeOffset.UtcNow.AddDays(-5), 89.99m, TransactionCurrency.RON, TransactionType.Debit, TransactionStatus.Successful, "Electronics For All", "Online purchase", "POS-40001"),
            new Transaction(Guid.NewGuid(), "acc-001", DateTimeOffset.UtcNow.AddDays(-6), 102.25m, TransactionCurrency.GBP, TransactionType.Credit, TransactionStatus.Successful, "CoffeeUnlimited LTD", "Coffee machine repair", "POS-10003"),
            new Transaction(Guid.NewGuid(), "acc-002", DateTimeOffset.UtcNow.AddDays(-7), 24.10m, TransactionCurrency.EUR, TransactionType.Credit, TransactionStatus.Successful, "CoffeeUnlimited LTD", "Espresso & water", "POS-10004"),
            new Transaction(Guid.NewGuid(), "acc-003", DateTimeOffset.UtcNow.AddDays(-8), 199.95m, TransactionCurrency.EUR, TransactionType.Debit, TransactionStatus.Failed, "Northwind Grocers", "Grocery basket (card declined)", "POS-20002"),
            new Transaction(Guid.NewGuid(), "acc-004", DateTimeOffset.UtcNow.AddDays(-9), 499.00m, TransactionCurrency.USD, TransactionType.Credit, TransactionStatus.Successful, "Tailspin Toys", "Gift purchase", "POS-50001"),
            new Transaction(Guid.NewGuid(), "acc-005", DateTimeOffset.UtcNow.AddDays(-10), 150.00m, TransactionCurrency.RON, TransactionType.Debit, TransactionStatus.Successful, "Electronics For All", "Headphones", "POS-40002"),
            new Transaction(Guid.NewGuid(), "acc-001", DateTimeOffset.UtcNow.AddDays(-11), 60.00m, TransactionCurrency.EUR, TransactionType.Credit, TransactionStatus.Successful, "CoffeeUnlimited LTD", "Latte", "POS-10005"),
            new Transaction(Guid.NewGuid(), "acc-002", DateTimeOffset.UtcNow.AddDays(-12), 300.00m, TransactionCurrency.USD, TransactionType.Credit, TransactionStatus.Successful, "Contoso Payroll", "Bonus payment", "POS-30002"),
            new Transaction(Guid.NewGuid(), "acc-003", DateTimeOffset.UtcNow.AddDays(-13), 45.00m, TransactionCurrency.GBP, TransactionType.Debit, TransactionStatus.Successful, "CoffeeUnlimited LTD", "Coffee beans", "POS-10006"),
            new Transaction(Guid.NewGuid(), "acc-004", DateTimeOffset.UtcNow.AddDays(-14), 80.00m, TransactionCurrency.RON, TransactionType.Debit, TransactionStatus.Successful, "Northwind Grocers", "Groceries", "POS-20003"),
            new Transaction(Guid.NewGuid(), "acc-005", DateTimeOffset.UtcNow.AddDays(-15), 250.00m, TransactionCurrency.USD, TransactionType.Credit, TransactionStatus.Successful, "Tailspin Toys", "Toy purchase", "POS-50002"),
            new Transaction(Guid.NewGuid(), "acc-001", DateTimeOffset.UtcNow.AddDays(-16), 35.00m, TransactionCurrency.EUR, TransactionType.Credit, TransactionStatus.Successful, "CoffeeUnlimited LTD", "Mocha", "POS-10007"),
            new Transaction(Guid.NewGuid(), "acc-002", DateTimeOffset.UtcNow.AddDays(-17), 120.00m, TransactionCurrency.RON, TransactionType.Debit, TransactionStatus.Successful, "Electronics For All", "Keyboard", "POS-40003"),
            new Transaction(Guid.NewGuid(), "acc-003", DateTimeOffset.UtcNow.AddDays(-18), 500.00m, TransactionCurrency.USD, TransactionType.Credit, TransactionStatus.Successful, "Contoso Payroll", "Salary payment", "POS-30003"),
            new Transaction(Guid.NewGuid(), "acc-004", DateTimeOffset.UtcNow.AddDays(-19), 20.00m, TransactionCurrency.EUR, TransactionType.Credit, TransactionStatus.Successful, "CoffeeUnlimited LTD", "Americano", "POS-10008"),
            new Transaction(Guid.NewGuid(), "acc-005", DateTimeOffset.UtcNow.AddDays(-20), 99.99m, TransactionCurrency.RON, TransactionType.Debit, TransactionStatus.Successful, "Electronics For All", "Mouse", "POS-40004"),
        };
    }
}
