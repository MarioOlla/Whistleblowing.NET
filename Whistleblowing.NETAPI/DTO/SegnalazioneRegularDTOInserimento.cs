using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Whistleblowing.NETAPI.DTO
{

	


	/// <summary>
	/// DTO segnalazione Regular per inserimento e modifica (privo di navigatori)
	/// <see cref="https://learn.microsoft.com/it-it/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5"/>
	/// </summary>
	[Table("segnalazioneRegulars")]
	public class SegnalazioneRegularDTOInserimento
	{
		[Column("segnalazione_regular_id")]
		public int Id { get; set; }

		[Column("fatto_riferito_a")]
		public string? FattoRiferitoA { get; set; }

		[Column("data_evento")]
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







	}
}
