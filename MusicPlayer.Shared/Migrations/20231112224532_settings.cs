using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlayer.Shared.Migrations
{
    /// <inheritdoc />
    public partial class settings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Volume = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SongsFolders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongsFolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SettingsSongsFolders",
                columns: table => new
                {
                    SettingsId = table.Column<int>(type: "INTEGER", nullable: false),
                    SongsFolderId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingsSongsFolders", x => new { x.SettingsId, x.SongsFolderId });
                    table.ForeignKey(
                        name: "FK_SettingsSongsFolders_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SettingsSongsFolders_SongsFolders_SongsFolderId",
                        column: x => x.SongsFolderId,
                        principalTable: "SongsFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SettingsSongsFolders_SongsFolderId",
                table: "SettingsSongsFolders",
                column: "SongsFolderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettingsSongsFolders");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "SongsFolders");
        }
    }
}
