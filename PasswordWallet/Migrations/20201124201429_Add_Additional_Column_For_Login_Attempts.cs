using Microsoft.EntityFrameworkCore.Migrations;

namespace PasswordWallet.Migrations
{
    public partial class Add_Additional_Column_For_Login_Attempts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WasSuccess",
                table: "LoginAttempts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WasSuccess",
                table: "LoginAttempts");
        }
    }
}
