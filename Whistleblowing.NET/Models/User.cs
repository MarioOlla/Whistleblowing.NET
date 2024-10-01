using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whistleblowing.NET.Models
{

    public class User
    {

       
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Cognome { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Azienda { get; set; }

        public string Posizione { get; set; }

        public string Telefono { get; set; }

        public DateTime DataNascita { get; set; }

        public string LuogoNascita { get; set; }

        public string Provincia { get; set; }

        public Boolean IsDeleted { get; set; }

        public Boolean IsLoggedIn { get; set; }

        

        public Ruolo? Ruolo { get; set; }
           
    }
}
