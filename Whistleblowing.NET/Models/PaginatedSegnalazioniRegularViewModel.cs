namespace Whistleblowing.NET.Models
{
    public class PaginatedSegnalazioniRegularViewModel
    {
        public int Id { get; set; }

        public DateTime DataEvento { get; set; }

        public string? SoggettoColpevole { get; set; }

        public List<SegnalazioneRegular> SeganalazioniRegulars { get; set; } = new List<SegnalazioneRegular>();

        //paginazione
        public List<int> number { get; set; } = new List<int> { 5, 10, 15 };
        public int numberSelected { get; set; } = 5;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public string SortBy { get; set; } = "Id_segnalazione";
        public bool SortDesc { get; set; } = true;
        //fine paginazione

    }
}
