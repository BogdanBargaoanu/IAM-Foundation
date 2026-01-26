using Microsoft.EntityFrameworkCore;
using TransactionsLibrary.Models;

namespace TransactionsApi.Data
{
    public sealed class TransactionsDbContext : DbContext
    {
        public TransactionsDbContext(DbContextOptions<TransactionsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>(mb =>
            {
                mb.ToTable("Transactions");

                mb.HasKey(t => t.Id);

                mb.Property(t => t.AccountId).HasMaxLength(64).IsRequired();
                mb.Property(t => t.Timestamp).IsRequired();
                mb.Property(t => t.Amount).HasColumnType("decimal(10,2)").IsRequired();

                mb.Property(t => t.Currency).HasConversion<int>().IsRequired();
                mb.Property(t => t.Type).HasConversion<int>().IsRequired();
                mb.Property(t => t.Status).HasConversion<int>().IsRequired();

                mb.Property(t => t.MerchantName).HasMaxLength(128);
                mb.Property(t => t.Description).HasMaxLength(1024);
                mb.Property(t => t.Reference).HasMaxLength(256);

                mb.HasIndex(t => t.Currency);
                mb.HasIndex(t => t.AccountId);
                mb.HasIndex(t => t.MerchantName);
                mb.HasIndex(t => t.Reference);
            });
        }
    }
}
