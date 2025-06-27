using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EComerceTestSamir.Migrations
{
    /// <inheritdoc />
    public partial class EditApplicationUserOTPTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "ApplicationUserOTPs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "ApplicationUserOTPs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "ApplicationUserOTPs");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "ApplicationUserOTPs");
        }
    }
}
