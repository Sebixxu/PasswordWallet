using Microsoft.EntityFrameworkCore.Migrations;

namespace PasswordWallet.Migrations
{
    public partial class Drop_FK_ForSharedPasswordId_Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingPasswordShares_Passwords_SharedPasswordId",
                table: "PendingPasswordShares");

            migrationBuilder.DropIndex(
                name: "IX_PendingPasswordShares_SharedPasswordId",
                table: "PendingPasswordShares");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
