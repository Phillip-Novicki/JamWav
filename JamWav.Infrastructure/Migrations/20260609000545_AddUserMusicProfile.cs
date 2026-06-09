using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamWav.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserMusicProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserMusicProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TopArtists = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TopTracks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TopGenres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpotifyUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpotifyAccessToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpotifyRefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMusicProfiles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMusicProfiles");
        }
    }
}
