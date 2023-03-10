using Microsoft.EntityFrameworkCore;
using mmTransactionDB.Models;

namespace mmTransactionDB.DataAccess
{
    public class mmTransactionDBContext : DbContext
    {
        public DbSet<mmTransaction> mmTransactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Token> Tokens { get; set; }

        public mmTransactionDBContext(DbContextOptions<mmTransactionDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wallet>()
                .HasKey(w => w.PublicKey);

            modelBuilder.Entity<Wallet>()
                .HasIndex(w => new { w.PrivateKey, w.PublicKey })
                .IsUnique(true);

            modelBuilder.Entity<Token>()
                .HasKey(t => t.PublicKey);

            modelBuilder.Entity<Token>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.Tokens)
                .HasForeignKey(k => k.OwnerId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<mmTransaction>()
                .HasKey(t => t.txId);

            base.OnModelCreating(modelBuilder);
        }
    }
}