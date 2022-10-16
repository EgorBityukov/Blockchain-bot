using Microsoft.EntityFrameworkCore;
using mmTransactionDB.Models;

namespace mmTransactionDB.DataAccess
{
    public class mmTransactionDBContext : DbContext
    {
        public DbSet<mmTransaction> mmTransactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

        public mmTransactionDBContext() : base()
        {
        }

        public mmTransactionDBContext(DbContextOptions<mmTransactionDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wallet>()
                .HasIndex(w => new { w.PrivateKey, w.PublicKey })
                .IsUnique(true);
        }
    }
}