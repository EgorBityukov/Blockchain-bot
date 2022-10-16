using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mmTransactionDB.Migrations
{
    public partial class UniqueKeysWallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Wallets_PrivateKey_PublicKey",
                table: "Wallets",
                columns: new[] { "PrivateKey", "PublicKey" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallets_PrivateKey_PublicKey",
                table: "Wallets");
        }
    }
}
