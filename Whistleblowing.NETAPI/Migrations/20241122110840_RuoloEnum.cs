using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whistleblowing.NETAPI.Migrations
{
    /// <inheritdoc />
    public partial class RuoloEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Ruolo_RuoloId",
                table: "User");

            migrationBuilder.DropTable(
                name: "Ruolo");

            migrationBuilder.DropIndex(
                name: "IX_User_RuoloId",
                table: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ruolo",
                columns: table => new
                {
                    ruolo_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codice = table.Column<int>(type: "int", nullable: false),
                    descrizione = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ruolo", x => x.ruolo_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_RuoloId",
                table: "User",
                column: "RuoloId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Ruolo_RuoloId",
                table: "User",
                column: "RuoloId",
                principalTable: "Ruolo",
                principalColumn: "ruolo_id");
        }
    }
}
