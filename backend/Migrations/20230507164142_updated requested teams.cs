using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class updatedrequestedteams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameTeam_Games_GamesRequestedToId",
                table: "GameTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_GameTeam_Teams_RequestedTeamsId",
                table: "GameTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamTournament_Teams_RequestedTeamsId",
                table: "TeamTournament");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamTournament_Tournaments_TournamentsRequestedToId",
                table: "TeamTournament");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamTournament",
                table: "TeamTournament");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameTeam",
                table: "GameTeam");

            migrationBuilder.RenameTable(
                name: "TeamTournament",
                newName: "TournamentRequestedTeams");

            migrationBuilder.RenameTable(
                name: "GameTeam",
                newName: "GameRequestedTeams");

            migrationBuilder.RenameIndex(
                name: "IX_TeamTournament_TournamentsRequestedToId",
                table: "TournamentRequestedTeams",
                newName: "IX_TournamentRequestedTeams_TournamentsRequestedToId");

            migrationBuilder.RenameIndex(
                name: "IX_GameTeam_RequestedTeamsId",
                table: "GameRequestedTeams",
                newName: "IX_GameRequestedTeams_RequestedTeamsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentRequestedTeams",
                table: "TournamentRequestedTeams",
                columns: new[] { "RequestedTeamsId", "TournamentsRequestedToId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameRequestedTeams",
                table: "GameRequestedTeams",
                columns: new[] { "GamesRequestedToId", "RequestedTeamsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GameRequestedTeams_Games_GamesRequestedToId",
                table: "GameRequestedTeams",
                column: "GamesRequestedToId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameRequestedTeams_Teams_RequestedTeamsId",
                table: "GameRequestedTeams",
                column: "RequestedTeamsId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentRequestedTeams_Teams_RequestedTeamsId",
                table: "TournamentRequestedTeams",
                column: "RequestedTeamsId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentRequestedTeams_Tournaments_TournamentsRequestedToId",
                table: "TournamentRequestedTeams",
                column: "TournamentsRequestedToId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameRequestedTeams_Games_GamesRequestedToId",
                table: "GameRequestedTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_GameRequestedTeams_Teams_RequestedTeamsId",
                table: "GameRequestedTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentRequestedTeams_Teams_RequestedTeamsId",
                table: "TournamentRequestedTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentRequestedTeams_Tournaments_TournamentsRequestedToId",
                table: "TournamentRequestedTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentRequestedTeams",
                table: "TournamentRequestedTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameRequestedTeams",
                table: "GameRequestedTeams");

            migrationBuilder.RenameTable(
                name: "TournamentRequestedTeams",
                newName: "TeamTournament");

            migrationBuilder.RenameTable(
                name: "GameRequestedTeams",
                newName: "GameTeam");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentRequestedTeams_TournamentsRequestedToId",
                table: "TeamTournament",
                newName: "IX_TeamTournament_TournamentsRequestedToId");

            migrationBuilder.RenameIndex(
                name: "IX_GameRequestedTeams_RequestedTeamsId",
                table: "GameTeam",
                newName: "IX_GameTeam_RequestedTeamsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamTournament",
                table: "TeamTournament",
                columns: new[] { "RequestedTeamsId", "TournamentsRequestedToId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameTeam",
                table: "GameTeam",
                columns: new[] { "GamesRequestedToId", "RequestedTeamsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GameTeam_Games_GamesRequestedToId",
                table: "GameTeam",
                column: "GamesRequestedToId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameTeam_Teams_RequestedTeamsId",
                table: "GameTeam",
                column: "RequestedTeamsId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamTournament_Teams_RequestedTeamsId",
                table: "TeamTournament",
                column: "RequestedTeamsId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamTournament_Tournaments_TournamentsRequestedToId",
                table: "TeamTournament",
                column: "TournamentsRequestedToId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
