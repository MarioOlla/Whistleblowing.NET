using System.ComponentModel.DataAnnotations;

namespace Whistleblowing.NET.Models.DTO
{
    public class UtenteDTOLogin
    {

        [Required(ErrorMessage = "L'email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "L'email non è valida.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La password è obbligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "La password deve essere almeno di 5 caratteri.")]
        public string Password { get; set; }
    }
}
