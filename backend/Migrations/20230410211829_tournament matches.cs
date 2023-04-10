using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class tournamentmatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamsBlockedGames");

            migrationBuilder.DropTable(
                name: "TeamsRequestedGames");

            migrationBuilder.AddColumn<Guid>(
                name: "GameId",
                table: "Teams",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePicture",
                table: "GameTeam",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "GameTeam",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TournamentMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Round = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    FirstParentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SecondParentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    TournamentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentMatches_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TournamentMatches_TournamentMatches_FirstParentId",
                        column: x => x.FirstParentId,
                        principalTable: "TournamentMatches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TournamentMatches_TournamentMatches_SecondParentId",
                        column: x => x.SecondParentId,
                        principalTable: "TournamentMatches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TournamentMatches_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_GameId",
                table: "Teams",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_FirstParentId",
                table: "TournamentMatches",
                column: "FirstParentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_GameId",
                table: "TournamentMatches",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_SecondParentId",
                table: "TournamentMatches",
                column: "SecondParentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_TournamentId",
                table: "TournamentMatches",
                column: "TournamentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Games_GameId",
                table: "Teams",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Games_GameId",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "TournamentMatches");

            migrationBuilder.DropIndex(
                name: "IX_Teams_GameId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Teams");

            migrationBuilder.UpdateData(
                table: "GameTeam",
                keyColumn: "ProfilePicture",
                keyValue: null,
                column: "ProfilePicture",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePicture",
                table: "GameTeam",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "GameTeam",
                keyColumn: "Description",
                keyValue: null,
                column: "Description",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "GameTeam",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TeamsBlockedGames",
                columns: table => new
                {
                    BlockedTeamsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GamesBlockedFromId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamsBlockedGames", x => new { x.BlockedTeamsId, x.GamesBlockedFromId });
                    table.ForeignKey(
                        name: "FK_TeamsBlockedGames_Games_GamesBlockedFromId",
                        column: x => x.GamesBlockedFromId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamsBlockedGames_Teams_BlockedTeamsId",
                        column: x => x.BlockedTeamsId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TeamsRequestedGames",
                columns: table => new
                {
                    GamesRequestedToId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RequestedTeamsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamsRequestedGames", x => new { x.GamesRequestedToId, x.RequestedTeamsId });
                    table.ForeignKey(
                        name: "FK_TeamsRequestedGames_Games_GamesRequestedToId",
                        column: x => x.GamesRequestedToId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamsRequestedGames_Teams_RequestedTeamsId",
                        column: x => x.RequestedTeamsId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TeamsBlockedGames_GamesBlockedFromId",
                table: "TeamsBlockedGames",
                column: "GamesBlockedFromId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamsRequestedGames_RequestedTeamsId",
                table: "TeamsRequestedGames",
                column: "RequestedTeamsId");
        }
    }
}
