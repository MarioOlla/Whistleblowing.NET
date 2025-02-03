using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Whistleblowing.NET.Models
{
    public class PaginatedSegnalazioniAnonimeViewModel
    {
        [JsonPropertyName("segnalazione_anonima_id")]

        [Column("segnalazione_anonima_id")]
        public int? segnalazione_anonima_id { get; set; }

        [Column("data_evento")]
        public DateTime? DataEvento { get; set; }

        [Column("soggetto_colpevole")]
        public string? SoggettoColpevole { get; set; }
        [Required]
        [EnumDataType(typeof(Status), ErrorMessage = "Lo status deve essere APERTO, LAVORAZIONE o CHIUSO.")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status? status { get; set; }


        // Aggiungi questa proprietà
        public List<PaginatedSegnalazioniAnonimeViewModel>? SegnalazioniAnonymous { get; set; }


        [NotMapped]
        public List<int> number { get; set; } = new List<int> { 5, 10, 15 };

        [NotMapped]
        public int numberselected { get; set; } = 5;

        [NotMapped]
        public int PageNumber { get; set; } = 1;

        [NotMapped]
        public int PageSize { get; set; } = 10;

        [NotMapped]
        public int TotalItems { get; set; }

        [NotMapped]
        public string sortby { get; set; } = "segnalazione_regular_id";

        [NotMapped]
        public bool sortdesc { get; set; } = true;

    }
}