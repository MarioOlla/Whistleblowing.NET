using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Whistleblowing.NET.Models
{
    public class PaginatedSegnalazioniRegularViewUtente
    {
        [JsonPropertyName("segnalazione_regular_id")]
        [Column("segnalazione_regular_id")]
        public int? segnalazione_regular_id { get; set; }


        [JsonPropertyName("dataEvento")]
        [Column("data_evento")]
        public DateTime? DataEvento { get; set; }


        [JsonPropertyName("soggettoColpevole")]
        [Column("soggetto_colpevole")]
        public string? SoggettoColpevole { get; set; }

        [JsonPropertyName("userId")]
        [Column("user_id")]
        public int UserId { get; set; }

        // Aggiungi questa proprietà
        public List<PaginatedSegnalazioniRegularViewUtente>? SegnalazioniRegulars { get; set; }


    }
}