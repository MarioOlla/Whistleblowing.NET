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

        // Proprietà per la paginazione
        public List<int> Number { get; set; } = new List<int> { 5, 10, 15 };

        [JsonPropertyName("numberSelected")]
        public int NumberSelected { get; set; } = 5;

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; } = 1;

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 10;

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("sortBy")]
        public string SortBy { get; set; } = "segnalazione_regular_id";

        [JsonPropertyName("sortDesc")]
        public bool SortDesc { get; set; } = true;
    }
}