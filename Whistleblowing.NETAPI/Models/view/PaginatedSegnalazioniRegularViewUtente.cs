using System.ComponentModel.DataAnnotations.Schema;

namespace Whistleblowing.NETAPI.Models.view
{
    public class PaginatedSegnalazioniRegularViewUtente
    {

        [Column("segnalazione_regular_id")]
        public int? segnalazione_regular_id { get; set; }

        [Column("data_evento")]
        public DateTime? DataEvento { get; set; }

        [Column("soggetto_colpevole")]
        public string? SoggettoColpevole { get; set; }


        [Column("user_id")]
        public int UserId { get; set; }

        // Aggiungi questa proprietà
        public List<PaginatedSegnalazioniRegularViewUtente>? SegnalazioniRegulars { get; set; }
    }
}