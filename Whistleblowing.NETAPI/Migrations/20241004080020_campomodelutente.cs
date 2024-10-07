using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whistleblowing.NETAPI.Migrations
{
    /// <inheritdoc />
    public partial class campomodelutente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasChangedPassword",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasChangedPassword",
                table: "User");
        }
    }
}
