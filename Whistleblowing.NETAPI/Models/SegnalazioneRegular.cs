﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Whistleblowing.NETAPI.Models
{
	public enum Status
	{

		APERTO,
		LAVORAZIONE,
		CHIUSO

	}

	public class SegnalazioneRegular
	{

		

		[Key]
		[Column("segnalazione_regular_id")]
		public int Id { get; set; }

		[Column("fatto_riferito_a")]
		public string? FattoRiferitoA { get; set; }

		[Column("data_evento")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]

		public DateTime? DataEvento { get; set; }

		[Column("luogo_evento")]
		public string? LuogoEvento { get; set; }

		[Column("soggetto_colpevole")]
		public string? SoggettoColpevole { get; set; }

		[Column("area_aziendale")]
		public string? AreaAziendale { get; set; }

		[Column("soggetti_privati_coinvolti")]
		public string? SoggettiPrivatiCoinvolti { get; set; }

		[Column("imprese_coinvolte")]
		public string? ImpreseCoinvolte { get; set; }

		[Column("pubblici_ufficiali_pa_coinvolti")]
		public string? PubbliciUfficialiPaCoinvolti { get; set; }

		[Column("modalita_conoscenza_fatto")]
		public string? ModalitaConoscenzaFatto { get; set; }

		[Column("soggetti_referenti_fatto")]
		public string? SoggettiReferentiFatto { get; set; }

		[Column("ammontare_pagamento_o_altra_utilita")]
		public string? AmmontarePagamentoOAltraUtilita { get; set; }

		[Column("circostanze_violenza_minaccia")]
		public string? CircostanzeViolenzaMinaccia { get; set; }

		[Column("descrizione_fatto")]
		public string? DescrizioneFatto { get; set; }

		[Column("motivazione_fatto_illecito")]
		public string? MotivazioneFattoIllecito { get; set; }

		[Column("note")]
		public string? Note { get; set; }

		[Column("status")]
		[JsonConverter(typeof(StringEnumConverter))]
		public Status? status { get; set; }


		[Column("is_deleted")]
		public Boolean? IsDeleted { get; set; }

		[ForeignKey("user_id"), Column("user_id")] 
		public int UserId { get; set; }




	}
}
