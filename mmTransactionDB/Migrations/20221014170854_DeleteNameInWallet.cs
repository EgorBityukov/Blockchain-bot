using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mmTransactionDB.Migrations
{
    public partial class DeleteNameInWallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Wallets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Wallets",
                type: "text",
                nullable: true);
        }
    }
}
