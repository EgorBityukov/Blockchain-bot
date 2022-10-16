using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mmTransactionDB.Migrations
{
    public partial class HotWalletModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HotWallet",
                table: "Wallets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Tokens",
                table: "Wallets",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HotWallet",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "Tokens",
                table: "Wallets");
        }
    }
}
