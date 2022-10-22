using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mmTransactionDB.Migrations
{
    public partial class ChangeKeyWalletPublicKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_Wallets_IdToken",
                table: "Tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets");

            migrationBuilder.AlterColumn<string>(
                name: "PublicKey",
                table: "Wallets",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Tokens",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets",
                column: "PublicKey");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_OwnerId",
                table: "Tokens",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Wallets_OwnerId",
                table: "Tokens",
                column: "OwnerId",
                principalTable: "Wallets",
                principalColumn: "PublicKey",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_Wallets_OwnerId",
                table: "Tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Tokens_OwnerId",
                table: "Tokens");

            migrationBuilder.AlterColumn<string>(
                name: "PublicKey",
                table: "Wallets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Tokens",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets",
                column: "IdWallet");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Wallets_IdToken",
                table: "Tokens",
                column: "IdToken",
                principalTable: "Wallets",
                principalColumn: "IdWallet",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
