using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whistleblowing.NET.Migrations
{
    /// <inheritdoc />
    public partial class campiUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "data_nascita",
                table: "User",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "luogo_nascita",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "provincia",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_nascita",
                table: "User");

            migrationBuilder.DropColumn(
                name: "luogo_nascita",
                table: "User");

            migrationBuilder.DropColumn(
                name: "provincia",
                table: "User");
        }
    }
}
