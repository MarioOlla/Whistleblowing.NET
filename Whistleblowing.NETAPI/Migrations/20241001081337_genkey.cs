using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whistleblowing.NETAPI.Migrations
{
    /// <inheritdoc />
    public partial class genkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CryptoKey",
                columns: table => new
                {
                    crypto_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EncryptedRsaPrivateKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RsaPublicKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AesKeySize = table.Column<int>(type: "int", nullable: false),
                    AesIterations = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoKey", x => x.crypto_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CryptoKey");
        }
    }
}
