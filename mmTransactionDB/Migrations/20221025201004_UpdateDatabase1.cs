using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mmTransactionDB.Migrations
{
    public partial class UpdateDatabase1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WalletAddress",
                table: "mmTransactions",
                newName: "SendWalletAddress");

            migrationBuilder.AddColumn<string>(
                name: "RecieveWalletAddress",
                table: "mmTransactions",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecieveWalletAddress",
                table: "mmTransactions");

            migrationBuilder.RenameColumn(
                name: "SendWalletAddress",
                table: "mmTransactions",
                newName: "WalletAddress");
        }
    }
}
