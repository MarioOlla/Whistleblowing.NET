using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whistleblowing.NETAPI.Migrations
{
    /// <inheritdoc />
    public partial class migrazionenuova : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "SegnalazioneAnonymous",
                columns: table => new
                {
                    segnalazione_regular_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fatto_riferito_a = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    data_evento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    luogo_evento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    soggetto_colpevole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    area_aziendale = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    soggetti_privati_coinvolti = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    imprese_coinvolte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pubblici_ufficiali_pa_coinvolti = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    modalita_conoscenza_fatto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    soggetti_referenti_fatto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ammontare_pagamento_o_altra_utilita = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    circostanze_violenza_minaccia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    descrizione_fatto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    motivazione_fatto_illecito = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: true),
                    is_external = table.Column<bool>(type: "bit", nullable: true),
                    user_hashed = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegnalazioneAnonymous", x => x.segnalazione_regular_id);
                });

            migrationBuilder.CreateTable(
                name: "segnalazioneRegulars",
                columns: table => new
                {
                    segnalazione_regular_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fatto_riferito_a = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    data_evento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    luogo_evento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    soggetto_colpevole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    area_aziendale = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    soggetti_privati_coinvolti = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    imprese_coinvolte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pubblici_ufficiali_pa_coinvolti = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    modalita_conoscenza_fatto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    soggetti_referenti_fatto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ammontare_pagamento_o_altra_utilita = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    circostanze_violenza_minaccia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    descrizione_fatto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    motivazione_fatto_illecito = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: true),
                    is_external = table.Column<bool>(type: "bit", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_segnalazioneRegulars", x => x.segnalazione_regular_id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cognome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    azienda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    posizione = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data_nascita = table.Column<DateTime>(type: "datetime2", nullable: false),
                    luogo_nascita = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    provincia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false),
                    isLoggedIn = table.Column<bool>(type: "bit", nullable: false),
                    RuoloId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_User_Ruolo_RuoloId",
                        column: x => x.RuoloId,
                        principalTable: "Ruolo",
                        principalColumn: "ruolo_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_RuoloId",
                table: "User",
                column: "RuoloId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SegnalazioneAnonymous");

            migrationBuilder.DropTable(
                name: "segnalazioneRegulars");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Ruolo");
        }
    }
}
