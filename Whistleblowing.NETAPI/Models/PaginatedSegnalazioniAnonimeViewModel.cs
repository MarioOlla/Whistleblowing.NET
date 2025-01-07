using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whistleblowing.NETAPI.Models
{
    public class PaginatedSegnalazioniAnonimeViewModel {


        [Column("Id_segnalazioneAnonima")]
        public int? Id_segnalazioneAnonima { get; set; }

        [Column("DataEvento")]
        public DateTime? DataEvento { get; set; }

        [Column("SoggettoColpevole")]
        public string? SoggettoColpevole { get; set; }

        [Column("is_deleted")]
        public bool? IsDeleted { get; set; }

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
        public string sortby { get; set; } = "Id_segnalazioneAnonima";

        [NotMapped]
        public bool sortdesc { get; set; } = true;


    }

}
