using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mmTransactionDB.Migrations
{
    public partial class start : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mmTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OperationType = table.Column<string>(type: "text", nullable: true),
                    WallerAddress = table.Column<string>(type: "text", nullable: true),
                    SendTokenCount = table.Column<double>(type: "double precision", nullable: false),
                    RecieveTokenName = table.Column<string>(type: "text", nullable: true),
                    RecieveTokenCount = table.Column<double>(type: "double precision", nullable: false),
                    BalanceXToken = table.Column<double>(type: "double precision", nullable: false),
                    BalanceUSDCToken = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mmTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    IdWallet = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicKey = table.Column<string>(type: "text", nullable: true),
                    PrivateKey = table.Column<string>(type: "text", nullable: true),
                    Lamports = table.Column<double>(type: "double precision", nullable: false),
                    SOL = table.Column<double>(type: "double precision", nullable: false),
                    ApproximateMintPrice = table.Column<double>(type: "double precision", nullable: false),
                    HotWallet = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.IdWallet);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    IdToken = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicKey = table.Column<string>(type: "text", nullable: true),
                    Mint = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.IdToken);
                    table.ForeignKey(
                        name: "FK_Tokens_Wallets_IdToken",
                        column: x => x.IdToken,
                        principalTable: "Wallets",
                        principalColumn: "IdWallet",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_PrivateKey_PublicKey",
                table: "Wallets",
                columns: new[] { "PrivateKey", "PublicKey" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mmTransactions");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "Wallets");
        }
    }
}
