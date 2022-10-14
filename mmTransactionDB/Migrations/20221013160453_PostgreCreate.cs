using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mmTransactionDB.Migrations
{
    public partial class PostgreCreate : Migration
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mmTransactions");
        }
    }
}
