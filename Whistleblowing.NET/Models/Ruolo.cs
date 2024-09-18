using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whistleblowing.NET.Models
{

    public class Ruolo
    {
        [Key]
        [Column("ruolo_id")]
        public int Id { get; set; }

        [Column("codice")]
        public int codice { get; set; }

        [Column("descrizione")]
        public string descrizione { get; set; }

    }
}
