using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamWav.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MapEventEntityCorrected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Events",
                newName: "Date");

            migrationBuilder.AddColumn<Guid>(
                name: "BandId",
                table: "Events",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Events",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Venue",
                table: "Events",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BandId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Venue",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Events",
                newName: "StartDate");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
