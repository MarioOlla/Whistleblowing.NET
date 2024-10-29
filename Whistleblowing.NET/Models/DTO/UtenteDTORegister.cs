using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whistleblowing.NET.Models.DTO
{
    public class UtenteDTORegister
    {
        [Required(ErrorMessage = "Il nome è obbligatorio.")]
        [StringLength(50, ErrorMessage = "Il nome non può superare i 50 caratteri.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Il cognome è obbligatorio.")]
        [StringLength(50, ErrorMessage = "Il cognome non può superare i 50 caratteri.")]
        public string Cognome { get; set; }

        [Required(ErrorMessage = "Il codice fiscale è obbligatorio.")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Il codice fiscale deve essere di 16 caratteri.")]
        public string CodiceFiscale { get; set; }

        [Required(ErrorMessage = "L'email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "L'email non è valida.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La password è obbligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "La password deve essere almeno di 5 caratteri.")]
        public string Password { get; set; }

        [StringLength(50, ErrorMessage = "Il nome dell'azienda non può superare i 50 caratteri.")]
        public string Azienda { get; set; }

        [StringLength(50, ErrorMessage = "La posizione non può superare i 50 caratteri.")]
        public string Posizione { get; set; }

        [Phone(ErrorMessage = "Il numero di telefono non è valido.")]
        [StringLength(15, ErrorMessage = "Il numero di telefono non può superare i 15 caratteri.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La data di nascita è obbligatoria.")]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1900-01-01", "2100-12-31", ErrorMessage = "La data di nascita deve essere valida.")]
        public DateTime DataNascita { get; set; }


        public bool HasChangedPassword { get; set; }

        [StringLength(50, ErrorMessage = "La provincia non può superare i 50 caratteri.")]
        public string LuogoNascita { get; set; }

        [StringLength(50, ErrorMessage = "La provincia non può superare i 50 caratteri.")]
        public string Provincia { get; set; }


    }
}
