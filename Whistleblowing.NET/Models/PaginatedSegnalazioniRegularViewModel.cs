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

        [JsonPropertyName("number")]
        public List<int> number { get; set; } = new List<int> { 5, 10, 15 };

        [JsonPropertyName("numberSelected")]
        public int numberselected { get; set; } = 5;

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; } = 1;

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 10;

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("sortBy")]
        public string sortby { get; set; } = "id_segnalazioneRegular";

        [JsonPropertyName("sortDesc")]
        public bool sortdesc { get; set; } = true;

        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

    }
}
