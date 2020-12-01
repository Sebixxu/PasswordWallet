using Microsoft.EntityFrameworkCore.Migrations;

namespace PasswordWallet.Migrations
{
    public partial class Add_IpAttemptsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdIpAttempt",
                table: "LoginAttempts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "IpAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IpAddress = table.Column<string>(nullable: false),
                    WasSuccess = table.Column<bool>(nullable: false),
                    IsStale = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpAttempts", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoginAttempts_IpAttempts_IdIpAttempt",
                table: "LoginAttempts");

            migrationBuilder.DropTable(
                name: "IpAttempts");

            migrationBuilder.DropIndex(
                name: "IX_LoginAttempts_IdIpAttempt",
                table: "LoginAttempts");

            migrationBuilder.DropColumn(
                name: "IdIpAttempt",
                table: "LoginAttempts");
        }
    }
}
