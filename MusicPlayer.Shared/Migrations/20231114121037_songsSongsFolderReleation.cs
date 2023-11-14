using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlayer.Shared.Migrations
{
    /// <inheritdoc />
    public partial class songsSongsFolderReleation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SongsFolderId",
                table: "Songs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Songs_SongsFolderId",
                table: "Songs",
                column: "SongsFolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_SongsFolders_SongsFolderId",
                table: "Songs",
                column: "SongsFolderId",
                principalTable: "SongsFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songs_SongsFolders_SongsFolderId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Songs_SongsFolderId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "SongsFolderId",
                table: "Songs");
        }
    }
}
