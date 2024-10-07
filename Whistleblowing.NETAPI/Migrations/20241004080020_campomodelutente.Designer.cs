﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Whistleblowing.NETAPI.Data;

#nullable disable

namespace Whistleblowing.NETAPI.Migrations
{
    [DbContext(typeof(WhistleBlowingContext))]
    [Migration("20241004080020_campomodelutente")]
    partial class campomodelutente
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Whistleblowing.NETAPI.Models.CryptoKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("crypto_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AesIterations")
                        .HasColumnType("int")
                        .HasColumnName("AesIterations");

                    b.Property<int>("AesKeySize")
                        .HasColumnType("int")
                        .HasColumnName("AesKeySize");

                    b.Property<string>("EncryptedRsaPrivateKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("EncryptedRsaPrivateKey");

                    b.Property<string>("RsaPublicKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("RsaPublicKey");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Salt");

                    b.HasKey("Id");

                    b.ToTable("CryptoKey");
                });

            modelBuilder.Entity("Whistleblowing.NETAPI.Models.Ruolo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ruolo_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("codice")
                        .HasColumnType("int")
                        .HasColumnName("codice");

                    b.Property<string>("descrizione")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("descrizione");

                    b.HasKey("Id");

                    b.ToTable("Ruolo");
                });

            modelBuilder.Entity("Whistleblowing.NETAPI.Models.SegnalazioneAnonymous", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("segnalazione_regular_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AmmontarePagamentoOAltraUtilita")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ammontare_pagamento_o_altra_utilita");

                    b.Property<string>("AreaAziendale")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("area_aziendale");

                    b.Property<string>("CircostanzeViolenzaMinaccia")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("circostanze_violenza_minaccia");

                    b.Property<DateTime?>("DataEvento")
                        .HasColumnType("datetime2")
                        .HasColumnName("data_evento");

                    b.Property<string>("DescrizioneFatto")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("descrizione_fatto");

                    b.Property<string>("FattoRiferitoA")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("fatto_riferito_a");

                    b.Property<string>("ImpreseCoinvolte")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("imprese_coinvolte");

                    b.Property<string>("LuogoEvento")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("luogo_evento");

                    b.Property<string>("ModalitaConoscenzaFatto")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("modalita_conoscenza_fatto");

                    b.Property<string>("MotivazioneFattoIllecito")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("motivazione_fatto_illecito");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("note");

                    b.Property<string>("PubbliciUfficialiPaCoinvolti")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("pubblici_ufficiali_pa_coinvolti");

                    b.Property<string>("SoggettiPrivatiCoinvolti")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("soggetti_privati_coinvolti");

                    b.Property<string>("SoggettiReferentiFatto")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("soggetti_referenti_fatto");

                    b.Property<string>("SoggettoColpevole")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("soggetto_colpevole");

                    b.Property<int>("UserHashed")
                        .HasColumnType("int")
                        .HasColumnName("user_hashed");

                    b.Property<int?>("status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.ToTable("SegnalazioneAnonymous");
                });

            modelBuilder.Entity("Whistleblowing.NETAPI.Models.SegnalazioneRegular", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("segnalazione_regular_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AmmontarePagamentoOAltraUtilita")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ammontare_pagamento_o_altra_utilita");

                    b.Property<string>("AreaAziendale")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("area_aziendale");

                    b.Property<string>("CircostanzeViolenzaMinaccia")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("circostanze_violenza_minaccia");

                    b.Property<DateTime?>("DataEvento")
                        .HasColumnType("datetime2")
                        .HasColumnName("data_evento");

                    b.Property<string>("DescrizioneFatto")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("descrizione_fatto");

                    b.Property<string>("FattoRiferitoA")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("fatto_riferito_a");

                    b.Property<string>("ImpreseCoinvolte")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("imprese_coinvolte");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("is_deleted");

                    b.Property<string>("LuogoEvento")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("luogo_evento");

                    b.Property<string>("ModalitaConoscenzaFatto")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("modalita_conoscenza_fatto");

                    b.Property<string>("MotivazioneFattoIllecito")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("motivazione_fatto_illecito");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("note");

                    b.Property<string>("PubbliciUfficialiPaCoinvolti")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("pubblici_ufficiali_pa_coinvolti");

                    b.Property<string>("SoggettiPrivatiCoinvolti")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("soggetti_privati_coinvolti");

                    b.Property<string>("SoggettiReferentiFatto")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("soggetti_referenti_fatto");

                    b.Property<string>("SoggettoColpevole")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("soggetto_colpevole");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.Property<int?>("status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.ToTable("segnalazioneRegulars");
                });

            modelBuilder.Entity("Whistleblowing.NETAPI.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Azienda")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("azienda");

                    b.Property<string>("CodiceFiscale")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("codiceFiscale");

                    b.Property<string>("Cognome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("cognome");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DataNascita")
                        .HasColumnType("datetime2")
                        .HasColumnName("data_nascita");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("HasChangedPassword")
                        .HasColumnType("bit")
                        .HasColumnName("HasChangedPassword");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("isDeleted");

                    b.Property<bool>("IsExternal")
                        .HasColumnType("bit")
                        .HasColumnName("isExternal");

                    b.Property<bool>("IsLoggedIn")
                        .HasColumnType("bit")
                        .HasColumnName("isLoggedIn");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("LuogoNascita")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("luogo_nascita");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("nome");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("password");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Posizione")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("posizione");

                    b.Property<string>("Provincia")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("provincia");

                    b.Property<int?>("RuoloId")
                        .HasColumnType("int");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("telefono");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RuoloId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Whistleblowing.NETAPI.Models.view.SegnalazioneRegularView", b =>
                {
                    b.Property<string>("AmmontarePagamentoOAltraUtilita")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ammontare_pagamento_o_altra_utilita");

                    b.Property<string>("AreaAziendale")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("area_aziendale");

                    b.Property<string>("CircostanzeViolenzaMinaccia")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("circostanze_violenza_minaccia");

                    b.Property<DateTime?>("DataEvento")
                        .HasColumnType("datetime2")
                        .HasColumnName("data_evento");

                    b.Property<string>("DescrizioneFatto")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("descrizione_fatto");

                    b.Property<string>("FattoRiferitoA")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("fatto_riferito_a");

                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("segnalazione_regular_id");

                    b.Property<string>("ImpreseCoinvolte")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("imprese_coinvolte");

                    b.Property<string>("LuogoEvento")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("luogo_evento");

                    b.Property<string>("ModalitaConoscenzaFatto")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("modalita_conoscenza_fatto");

                    b.Property<string>("MotivazioneFattoIllecito")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("motivazione_fatto_illecito");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("note");

                    b.Property<string>("PubbliciUfficialiPaCoinvolti")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("pubblici_ufficiali_pa_coinvolti");

                    b.Property<string>("SoggettiPrivatiCoinvolti")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("soggetti_privati_coinvolti");

                    b.Property<string>("SoggettiReferentiFatto")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("soggetti_referenti_fatto");

                    b.Property<string>("SoggettoColpevole")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("soggetto_colpevole");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.Property<int?>("status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.ToTable((string)null);

                    b.ToView("SegnalazioneRegularView", (string)null);
                });

            modelBuilder.Entity("Whistleblowing.NETAPI.Models.User", b =>
                {
                    b.HasOne("Whistleblowing.NETAPI.Models.Ruolo", "Ruolo")
                        .WithMany()
                        .HasForeignKey("RuoloId");

                    b.Navigation("Ruolo");
                });
#pragma warning restore 612, 618
        }
    }
}
