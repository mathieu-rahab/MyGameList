using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mygamelist.DatabaseRepository.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesAndUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGames");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Friendships",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_User1Id_User2Id",
                table: "Friendships",
                columns: new[] { "User1Id", "User2Id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Friendships_User1Id_User2Id",
                table: "Friendships");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Friendships",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateTable(
                name: "UserGames",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    LastSync = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PlaytimeMinutes = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGames", x => new { x.UserId, x.GameId });
                    table.ForeignKey(
                        name: "FK_UserGames_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
