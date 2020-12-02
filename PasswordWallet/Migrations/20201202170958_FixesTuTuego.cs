using Microsoft.EntityFrameworkCore.Migrations;

namespace PasswordWallet.Migrations
{
    public partial class FixesTuTuego : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoginAttempts_IpAttempts_IdIpAttempt",
                table: "LoginAttempts");

            migrationBuilder.DropIndex(
                name: "IX_LoginAttempts_IdIpAttempt",
                table: "LoginAttempts");

            migrationBuilder.DropColumn(
                name: "IdIpAttempt",
                table: "LoginAttempts");

            migrationBuilder.AddColumn<int>(
                name: "IdUser",
                table: "IpAttempts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_IpAttempts_IdUser",
                table: "IpAttempts",
                column: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_IpAttempts_Users_IdUser",
                table: "IpAttempts",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IpAttempts_Users_IdUser",
                table: "IpAttempts");

            migrationBuilder.DropIndex(
                name: "IX_IpAttempts_IdUser",
                table: "IpAttempts");

            migrationBuilder.DropColumn(
                name: "IdUser",
                table: "IpAttempts");

            migrationBuilder.AddColumn<int>(
                name: "IdIpAttempt",
                table: "LoginAttempts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempts_IdIpAttempt",
                table: "LoginAttempts",
                column: "IdIpAttempt");

            migrationBuilder.AddForeignKey(
                name: "FK_LoginAttempts_IpAttempts_IdIpAttempt",
                table: "LoginAttempts",
                column: "IdIpAttempt",
                principalTable: "IpAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
