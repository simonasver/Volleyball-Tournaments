using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class addedadditionalvalues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Tournaments",
                newName: "PointsToWinLastSet");

            migrationBuilder.AddColumn<bool>(
                name: "Basic",
                table: "Tournaments",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SingleThirdPlace",
                table: "Tournaments",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<uint>(
                name: "Score",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<uint>(
                name: "Aces",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "Attempts",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "BallMisses",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "BallTouches",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "BlockingErrors",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "Blocks",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "Errors",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "Kills",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "ServingErrors",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "SuccessfulBlocks",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "SuccessfulDigs",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "TotalServes",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "Touches",
                table: "SetPlayers",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Basic",
                table: "Games",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PointsToWinLastSet",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Basic",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "SingleThirdPlace",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "Aces",
                table: "SetPlayers");

            migrationBuilder.DropColumn(
                name: "Attempts",
                table: "SetPlayers");

            migrationBuilder.DropColumn(
                name: "BallMisses",
                table: "SetPlayers");

            migrationBuilder.DropColumn(
                name: "BallTouches",
                table: "SetPlayers");

            migrationBuilder.DropColumn(
                name: "BlockingErrors",
                table: "SetPlayers");

            migrationBuilder.DropColumn(
                name: "Blocks",
                table: "SetPlayers");

            migrationBuilder.DropColumn(
                name: "Errors",
                table: "SetPlayers");

            migrationBuilder.DropColumn(
                name: "Kills",
                table: "SetPlayers");

            migrationBuilder.DropColumn(
                name: "ServingErrors",
                table: "SetPlayers");

            migrationBuilder.DropColumn(
                name: "SuccessfulBlocks",
                table: "SetPlayers");

            migrationBuilder.DropColumn(
                name: "SuccessfulDigs",
                table: "SetPlayers");

            migrationBuilder.DropColumn(
                name: "TotalServes",
                table: "SetPlayers");

            migrationBuilder.DropColumn(
                name: "Touches",
                table: "SetPlayers");

            migrationBuilder.DropColumn(
                name: "Basic",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PointsToWinLastSet",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "PointsToWinLastSet",
                table: "Tournaments",
                newName: "Type");

            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "SetPlayers",
                type: "int",
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "int unsigned");
        }
    }
}
