using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whistleblowing.NETAPI.Migrations
{
    /// <inheritdoc />
    public partial class modifichemodelsegnanonim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "segnalazione_regular_id",
                table: "SegnalazioneAnonymous",
                newName: "segnalazione_anonima_id");

            migrationBuilder.AlterColumn<string>(
                name: "user_hashed",
                table: "SegnalazioneAnonymous",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "segnalazione_anonima_id",
                table: "SegnalazioneAnonymous",
                newName: "segnalazione_regular_id");

            migrationBuilder.AlterColumn<int>(
                name: "user_hashed",
                table: "SegnalazioneAnonymous",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
