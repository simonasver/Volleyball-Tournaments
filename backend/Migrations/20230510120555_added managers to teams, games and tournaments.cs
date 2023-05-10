using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class addedmanagerstoteamsgamesandtournaments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameManagers",
                columns: table => new
                {
                    ManagedGamesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ManagersId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameManagers", x => new { x.ManagedGamesId, x.ManagersId });
                    table.ForeignKey(
                        name: "FK_GameManagers_AspNetUsers_ManagersId",
                        column: x => x.ManagersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameManagers_Games_ManagedGamesId",
                        column: x => x.ManagedGamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TeamManagers",
                columns: table => new
                {
                    ManagedTeamsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ManagersId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamManagers", x => new { x.ManagedTeamsId, x.ManagersId });
                    table.ForeignKey(
                        name: "FK_TeamManagers_AspNetUsers_ManagersId",
                        column: x => x.ManagersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamManagers_Teams_ManagedTeamsId",
                        column: x => x.ManagedTeamsId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TournamentManagers",
                columns: table => new
                {
                    ManagedTournamentsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ManagersId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentManagers", x => new { x.ManagedTournamentsId, x.ManagersId });
                    table.ForeignKey(
                        name: "FK_TournamentManagers_AspNetUsers_ManagersId",
                        column: x => x.ManagersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TournamentManagers_Tournaments_ManagedTournamentsId",
                        column: x => x.ManagedTournamentsId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GameManagers_ManagersId",
                table: "GameManagers",
                column: "ManagersId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamManagers_ManagersId",
                table: "TeamManagers",
                column: "ManagersId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentManagers_ManagersId",
                table: "TournamentManagers",
                column: "ManagersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameManagers");

            migrationBuilder.DropTable(
                name: "TeamManagers");

            migrationBuilder.DropTable(
                name: "TournamentManagers");
        }
    }
}
