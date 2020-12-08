using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PasswordWallet.Migrations
{
    public partial class Add_PendingPasswordShares : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PendingPasswordShares",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordId = table.Column<int>(nullable: false),
                    SourceUserId = table.Column<int>(nullable: false),
                    DestinationUserId = table.Column<int>(nullable: false),
                    IsStale = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingPasswordShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingPasswordShares_Users_DestinationUserId",
                        column: x => x.DestinationUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PendingPasswordShares_Passwords_PasswordId",
                        column: x => x.PasswordId,
                        principalTable: "Passwords",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PendingPasswordShares_Users_SourceUserId",
                        column: x => x.SourceUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PendingPasswordShares_DestinationUserId",
                table: "PendingPasswordShares",
                column: "DestinationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingPasswordShares_PasswordId",
                table: "PendingPasswordShares",
                column: "PasswordId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingPasswordShares_SourceUserId",
                table: "PendingPasswordShares",
                column: "SourceUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingPasswordShares");
        }
    }
}
