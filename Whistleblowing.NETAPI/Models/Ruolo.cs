using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Whistleblowing.NETAPI.Models
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
