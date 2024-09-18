using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whistleblowing.NET.Migrations
{
    /// <inheritdoc />
    public partial class userHashed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "SegnalazioneAnonymous",
                newName: "user_hashed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_hashed",
                table: "SegnalazioneAnonymous",
                newName: "user_id");
        }
    }
}
