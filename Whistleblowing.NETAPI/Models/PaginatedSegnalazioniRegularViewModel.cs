using Whistleblowing.NETAPI.Models;

namespace Whistleblowing.NET.Models
{
    public class PaginatedSegnalazioniRegularViewModel
    {

        public int Id_segnalazioneRegular { get; set; }
        public DateTime DataEvento { get; set; }
        public string? SoggettoColpevole { get; set; } 
        public List<SegnalazioneRegular> SegnalazioniRegulars { get; set; } // Questo rappresenta la lista delle segnalazioni

        // Paginazione
        public List<int> Number { get; set; } = new List<int> { 5, 10, 15 }; // Opzioni per la selezione del numero di elementi per pagina
        public int NumberSelected { get; set; } = 5; // Numero di elementi selezionati per pagina
        public int PageNumber { get; set; } = 1; // Numero della pagina corrente
        public int PageSize { get; set; } = 10; // Dimensione della pagina
        public int TotalItems { get; set; } // Numero totale di segnalazioni
        public string SortBy { get; set; } = "Id_segnalazione"; // Campo per ordinamento
        public bool SortDesc { get; set; } = true; // Ordinamento decrescente o crescente
        // Fine paginazione
    }
}
