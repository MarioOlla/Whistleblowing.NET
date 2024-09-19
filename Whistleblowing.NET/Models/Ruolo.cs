using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whistleblowing.NET.Models
{

    public class Ruolo
    {
       
        public int Id { get; set; }

      
        public int codice { get; set; }

        public string descrizione { get; set; }

    }
}
