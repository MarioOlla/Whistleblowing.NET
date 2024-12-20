using System.ComponentModel.DataAnnotations.Schema;

namespace Whistleblowing.NETAPI.Models.view
{
    public class PaginatedSegnalazioniAnonimeViewUtente
    {

        [Column("segnalazione_anonima_id")]
        public int? segnalazione_anonima_id { get; set; }

        [Column("data_evento")]
        public DateTime? DataEvento { get; set; }

        [Column("soggetto_colpevole")]
        public string? SoggettoColpevole { get; set; }

        // Aggiungi questa proprietà
        public List<PaginatedSegnalazioniAnonimeViewUtente>? SegnalazioniAnonime { get; set; }


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
