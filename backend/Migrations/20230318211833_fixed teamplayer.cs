using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class fixedteamplayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamPlayer_Teams_TeamId",
                table: "TeamPlayer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamPlayer",
                table: "TeamPlayer");

            migrationBuilder.RenameTable(
                name: "TeamPlayer",
                newName: "TeamPlayers");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Teams",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_TeamPlayer_TeamId",
                table: "TeamPlayers",
                newName: "IX_TeamPlayers_TeamId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Teams",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditDate",
                table: "Teams",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Teams",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeamId",
                table: "TeamPlayers",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamPlayers",
                table: "TeamPlayers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamPlayers_Teams_TeamId",
                table: "TeamPlayers",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamPlayers_Teams_TeamId",
                table: "TeamPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamPlayers",
                table: "TeamPlayers");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "LastEditDate",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Teams");

            migrationBuilder.RenameTable(
                name: "TeamPlayers",
                newName: "TeamPlayer");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Teams",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_TeamPlayers_TeamId",
                table: "TeamPlayer",
                newName: "IX_TeamPlayer_TeamId");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeamId",
                table: "TeamPlayer",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamPlayer",
                table: "TeamPlayer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamPlayer_Teams_TeamId",
                table: "TeamPlayer",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }
    }
}
