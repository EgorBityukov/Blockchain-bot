// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using mmTransactionDB.DataAccess;

#nullable disable

namespace mmTransactionDB.Migrations
{
    [DbContext(typeof(mmTransactionDBContext))]
    [Migration("20221024131225_DeleteIdWallet")]
    partial class DeleteIdWallet
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("mmTransactionDB.Models.mmTransaction", b =>
                {
                    b.Property<string>("txId")
                        .HasColumnType("text");

                    b.Property<double>("BalanceUSDCToken")
                        .HasColumnType("double precision");

                    b.Property<double>("BalanceXToken")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("OperationType")
                        .HasColumnType("text");

                    b.Property<double>("RecieveTokenCount")
                        .HasColumnType("double precision");

                    b.Property<string>("RecieveTokenMint")
                        .HasColumnType("text");

                    b.Property<double>("SendTokenCount")
                        .HasColumnType("double precision");

                    b.Property<string>("SendTokenMint")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<string>("WalletAddress")
                        .HasColumnType("text");

                    b.HasKey("txId");

                    b.ToTable("mmTransactions");
                });

            modelBuilder.Entity("mmTransactionDB.Models.Token", b =>
                {
                    b.Property<Guid>("IdToken")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Amount")
                        .HasColumnType("text");

                    b.Property<double>("AmountDouble")
                        .HasColumnType("double precision");

                    b.Property<string>("Mint")
                        .HasColumnType("text");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PublicKey")
                        .HasColumnType("text");

                    b.HasKey("IdToken");

                    b.HasIndex("OwnerId");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("mmTransactionDB.Models.Wallet", b =>
                {
                    b.Property<string>("PublicKey")
                        .HasColumnType("text");

                    b.Property<double>("ApproximateMintPrice")
                        .HasColumnType("double precision");

                    b.Property<bool>("HotWallet")
                        .HasColumnType("boolean");

                    b.Property<long>("Lamports")
                        .HasColumnType("bigint");

                    b.Property<string>("PrivateKey")
                        .HasColumnType("text");

                    b.Property<double>("SOL")
                        .HasColumnType("double precision");

                    b.HasKey("PublicKey");

                    b.HasIndex("PrivateKey", "PublicKey")
                        .IsUnique();

                    b.ToTable("Wallets");
                });

            modelBuilder.Entity("mmTransactionDB.Models.Token", b =>
                {
                    b.HasOne("mmTransactionDB.Models.Wallet", "Owner")
                        .WithMany("Tokens")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("mmTransactionDB.Models.Wallet", b =>
                {
                    b.Navigation("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}
