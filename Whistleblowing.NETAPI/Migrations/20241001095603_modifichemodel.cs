using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whistleblowing.NETAPI.Migrations
{
    /// <inheritdoc />
    public partial class modifichemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_external",
                table: "segnalazioneRegulars");

            migrationBuilder.DropColumn(
                name: "is_external",
                table: "SegnalazioneAnonymous");

            migrationBuilder.AddColumn<bool>(
                name: "isExternal",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isExternal",
                table: "User");

            migrationBuilder.AddColumn<bool>(
                name: "is_external",
                table: "segnalazioneRegulars",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_external",
                table: "SegnalazioneAnonymous",
                type: "bit",
                nullable: true);
        }
    }
}
