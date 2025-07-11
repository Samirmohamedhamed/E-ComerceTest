﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EComerceTestSamir.Migrations
{
    /// <inheritdoc />
    public partial class addTrafficcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Traffic",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Traffic",
                table: "Products");
        }
    }
}
