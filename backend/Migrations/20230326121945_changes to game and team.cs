using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class changestogameandteam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "Teams",
                newName: "CreateDate");

            migrationBuilder.CreateTable(
                name: "GameTeam",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProfilePicture = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTeam", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PointsToWin = table.Column<int>(type: "int", nullable: false),
                    PointDifferenceToWin = table.Column<int>(type: "int", nullable: false),
                    SetsToWin = table.Column<int>(type: "int", nullable: false),
                    PlayersPerTeam = table.Column<int>(type: "int", nullable: false),
                    FirstTeamId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SecondTeamId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsPrivate = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastEditDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WinnerId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    FinishDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    OwnerId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_GameTeam_FirstTeamId",
                        column: x => x.FirstTeamId,
                        principalTable: "GameTeam",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Games_GameTeam_SecondTeamId",
                        column: x => x.SecondTeamId,
                        principalTable: "GameTeam",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Games_GameTeam_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "GameTeam",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GameTeamPlayer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GameTeamId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTeamPlayer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameTeamPlayer_GameTeam_GameTeamId",
                        column: x => x.GameTeamId,
                        principalTable: "GameTeam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FirstTeamId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SecondTeamId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FirstTeamScore = table.Column<int>(type: "int", nullable: false),
                    SecondTeamScore = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    WinnerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FinishDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GameId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sets_GameTeam_FirstTeamId",
                        column: x => x.FirstTeamId,
                        principalTable: "GameTeam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sets_GameTeam_SecondTeamId",
                        column: x => x.SecondTeamId,
                        principalTable: "GameTeam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sets_GameTeam_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "GameTeam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sets_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateTable(
                name: "SetPlayer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Team = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SetId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetPlayer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetPlayer_Sets_SetId",
                        column: x => x.SetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Games_FirstTeamId",
                table: "Games",
                column: "FirstTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_OwnerId",
                table: "Games",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_SecondTeamId",
                table: "Games",
                column: "SecondTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_WinnerId",
                table: "Games",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTeamPlayer_GameTeamId",
                table: "GameTeamPlayer",
                column: "GameTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SetPlayer_SetId",
                table: "SetPlayer",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_FirstTeamId",
                table: "Sets",
                column: "FirstTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_GameId",
                table: "Sets",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_SecondTeamId",
                table: "Sets",
                column: "SecondTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_WinnerId",
                table: "Sets",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamsBlockedGames_GamesBlockedFromId",
                table: "TeamsBlockedGames",
                column: "GamesBlockedFromId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamsRequestedGames_RequestedTeamsId",
                table: "TeamsRequestedGames",
                column: "RequestedTeamsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameTeamPlayer");

            migrationBuilder.DropTable(
                name: "SetPlayer");

            migrationBuilder.DropTable(
                name: "TeamsBlockedGames");

            migrationBuilder.DropTable(
                name: "TeamsRequestedGames");

            migrationBuilder.DropTable(
                name: "Sets");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "GameTeam");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Teams",
                newName: "CreationDate");
        }
    }
}
