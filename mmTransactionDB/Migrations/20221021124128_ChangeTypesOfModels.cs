using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mmTransactionDB.Migrations
{
    public partial class ChangeTypesOfModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WallerAddress",
                table: "mmTransactions",
                newName: "txId");

            migrationBuilder.RenameColumn(
                name: "RecieveTokenName",
                table: "mmTransactions",
                newName: "WalletAddress");

            migrationBuilder.AlterColumn<long>(
                name: "Lamports",
                table: "Wallets",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<double>(
                name: "AmountDouble",
                table: "Tokens",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "mmTransactions",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "RecieveTokenMint",
                table: "mmTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SendTokenMint",
                table: "mmTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "mmTransactions",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountDouble",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "RecieveTokenMint",
                table: "mmTransactions");

            migrationBuilder.DropColumn(
                name: "SendTokenMint",
                table: "mmTransactions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "mmTransactions");

            migrationBuilder.RenameColumn(
                name: "txId",
                table: "mmTransactions",
                newName: "WallerAddress");

            migrationBuilder.RenameColumn(
                name: "WalletAddress",
                table: "mmTransactions",
                newName: "RecieveTokenName");

            migrationBuilder.AlterColumn<double>(
                name: "Lamports",
                table: "Wallets",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "mmTransactions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
