using Microsoft.EntityFrameworkCore.Migrations;

namespace PasswordWallet.Migrations
{
    public partial class Add_SharedPasswordId_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SharedPasswordId",
                table: "PendingPasswordShares",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PendingPasswordShares_SharedPasswordId",
                table: "PendingPasswordShares",
                column: "SharedPasswordId");

            migrationBuilder.AddForeignKey(
                name: "FK_PendingPasswordShares_Passwords_SharedPasswordId",
                table: "PendingPasswordShares",
                column: "SharedPasswordId",
                principalTable: "Passwords",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingPasswordShares_Passwords_SharedPasswordId",
                table: "PendingPasswordShares");

            migrationBuilder.DropIndex(
                name: "IX_PendingPasswordShares_SharedPasswordId",
                table: "PendingPasswordShares");

            migrationBuilder.DropColumn(
                name: "SharedPasswordId",
                table: "PendingPasswordShares");
        }
    }
}
