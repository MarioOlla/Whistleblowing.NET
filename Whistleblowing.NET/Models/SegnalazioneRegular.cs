using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Whistleblowing.NET.Models
{
    public enum Status
    {
        APERTO, LAVORAZIONE, CHIUSO
    }
    public class SegnalazioneRegular
    {


        [JsonPropertyName("segnalazione_regular_id")]
        public int Id { get; set; }

        [JsonPropertyName("fatto_riferito_a")]
        public string? FattoRiferitoA { get; set; }

        [JsonPropertyName("data_evento")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataEvento { get; set; }

        [JsonPropertyName("luogo_evento")]
        public string? LuogoEvento { get; set; }

        [JsonPropertyName("soggetto_colpevole")]
        public string? SoggettoColpevole { get; set; }

        [JsonPropertyName("area_aziendale")]
        public string? AreaAziendale { get; set; }

        [JsonPropertyName("soggetti_privati_coinvolti")]
        public string? SoggettiPrivatiCoinvolti { get; set; }

        [JsonPropertyName("imprese_coinvolte")]
        public string? ImpreseCoinvolte { get; set; }

        [JsonPropertyName("pubblici_ufficiali_pa_coinvolti")]
        public string? PubbliciUfficialiPaCoinvolti { get; set; }

        [JsonPropertyName("modalita_conoscenza_fatto")]
        public string? ModalitaConoscenzaFatto { get; set; }

        [JsonPropertyName("soggetti_referenti_fatto")]
        public string? SoggettiReferentiFatto { get; set; }

        [JsonPropertyName("ammontare_pagamento_o_altra_utilita")]
        public string? AmmontarePagamentoOAltraUtilita { get; set; }

        [JsonPropertyName("circostanze_violenza_minaccia")]
        public string? CircostanzeViolenzaMinaccia { get; set; }

        [JsonPropertyName("descrizione_fatto")]
        public string? DescrizioneFatto { get; set; }

        [JsonPropertyName("motivazione_fatto_illecito")]
        public string? MotivazioneFattoIllecito { get; set; }

        [JsonPropertyName("note")]
        public string? Note { get; set; }

        [JsonPropertyName("status")]
        public Status? status { get; set; }

        [JsonPropertyName("is_deleted")]
        public Boolean? IsDeleted { get; set; }


        [JsonPropertyName("user_id")]
        public int UserId { get; set; }




    }
}























