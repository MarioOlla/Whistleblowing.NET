using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Whistleblowing.NET.Models
{
    public class PaginatedSegnalazioniRegularViewModel
    {
        [JsonPropertyName("id_segnalazioneRegular")]
        [Column("Id_segnalazioneRegular")]
        public int? Id_segnalazioneRegular { get; set; }


        [JsonPropertyName("dataEvento")]
        [Column("DataEvento")]
        public DateTime? DataEvento { get; set; }


        [JsonPropertyName("soggettoColpevole")]
        [Column("SoggettoColpevole")]
        public string? SoggettoColpevole { get; set; }

        // Aggiungi questa proprietà
        public List<PaginatedSegnalazioniRegularViewModel>? SegnalazioniRegulars { get; set; }
        //paginazione
        //public List<int> number { get; set; } = new List<int> { 5, 10, 15 };
        //public int numberSelected { get; set; } = 5;
        //public int PageNumber { get; set; } = 1;
        //public int PageSize { get; set; } = 10;
        //public int TotalItems { get; set; }
        //public string SortBy { get; set; } = "Id_segnalazione";
        //public bool SortDesc { get; set; } = true;
        //fine paginazione

    }
}
