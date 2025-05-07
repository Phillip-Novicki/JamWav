using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamWav.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendNameToFriend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FriendName",
                table: "Friends",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendName",
                table: "Friends");
        }
    }
}
