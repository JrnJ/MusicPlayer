using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlayer.Shared.Migrations
{
    /// <inheritdoc />
    public partial class playlistIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "PlaylistSongs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Index",
                table: "PlaylistSongs");
        }
    }
}
