using System.ComponentModel.DataAnnotations.Schema;
using Whistleblowing.NETAPI.Models.view;

namespace Whistleblowing.NETAPI.Models
{
    public class PaginatedSegnalazioniRegularViewModel
    {
        [Column("Id_segnalazioneRegular")]
        public int? Id_segnalazioneRegular { get; set; }
     
        [Column("DataEvento")]
        public DateTime? DataEvento { get; set; }

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
