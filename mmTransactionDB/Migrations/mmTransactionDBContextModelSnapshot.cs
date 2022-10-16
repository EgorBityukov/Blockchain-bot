﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using mmTransactionDB.DataAccess;

#nullable disable

namespace mmTransactionDB.Migrations
{
    [DbContext(typeof(mmTransactionDBContext))]
    partial class mmTransactionDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("mmTransactionDB.Models.mmTransaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

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

                    b.Property<string>("RecieveTokenName")
                        .HasColumnType("text");

                    b.Property<double>("SendTokenCount")
                        .HasColumnType("double precision");

                    b.Property<string>("WallerAddress")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("mmTransactions");
                });

            modelBuilder.Entity("mmTransactionDB.Models.Wallet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("ApproximateMintPrice")
                        .HasColumnType("double precision");

                    b.Property<bool>("HotWallet")
                        .HasColumnType("boolean");

                    b.Property<double>("Lamports")
                        .HasColumnType("double precision");

                    b.Property<string>("PrivateKey")
                        .HasColumnType("text");

                    b.Property<string>("PublicKey")
                        .HasColumnType("text");

                    b.Property<double>("SOL")
                        .HasColumnType("double precision");

                    b.Property<double>("Tokens")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("PrivateKey", "PublicKey")
                        .IsUnique();

                    b.ToTable("Wallets");
                });
#pragma warning restore 612, 618
        }
    }
}
