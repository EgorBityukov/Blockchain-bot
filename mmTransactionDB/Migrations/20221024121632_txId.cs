using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mmTransactionDB.Migrations
{
    public partial class txId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_mmTransactions",
                table: "mmTransactions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "mmTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "txId",
                table: "mmTransactions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_mmTransactions",
                table: "mmTransactions",
                column: "txId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_mmTransactions",
                table: "mmTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "txId",
                table: "mmTransactions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "mmTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mmTransactions",
                table: "mmTransactions",
                column: "Id");
        }
    }
}
