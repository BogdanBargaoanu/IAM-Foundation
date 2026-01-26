using Microsoft.EntityFrameworkCore;
using TransactionsApi.Data;
using TransactionsApi.Mock;

namespace TransactionsApi
{
    public static class SeedData
    {
        public static void EnsureSeedData(WebApplication app)
        {
            using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger(typeof(SeedData).FullName ?? nameof(SeedData));

            var db = scope.ServiceProvider.GetRequiredService<TransactionsDbContext>();

            db.Database.Migrate();

            if (!db.Transactions.Any())
            {
                logger.LogInformation("Seeding transactions");
                db.Transactions.AddRange(MockData.Transactions);
                db.SaveChanges();
                logger.LogInformation("Transactions populated: {Count}", MockData.Transactions.Count);
            }
            else
            {
                logger.LogInformation("Transactions already populated");
            }
        }
    }
}
