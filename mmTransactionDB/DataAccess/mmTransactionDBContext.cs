﻿using Microsoft.EntityFrameworkCore;
using mmTransactionDB.Models;

namespace mmTransactionDB.DataAccess
{
    public class mmTransactionDBContext : DbContext
    {
        public DbSet<mmTransaction> mmTransactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Token> Tokens { get; set; }

        public mmTransactionDBContext() : base()
        {
        }

        public mmTransactionDBContext(DbContextOptions<mmTransactionDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wallet>()
                .HasKey(w => w.IdWallet);
            modelBuilder.Entity<Wallet>()
                .HasIndex(w => new { w.PrivateKey, w.PublicKey })
                .IsUnique(true);
            modelBuilder.Entity<Token>()
                .HasKey(t => t.IdToken);
            modelBuilder.Entity<Token>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.Tokens)
                .HasForeignKey(k => k.IdToken)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}