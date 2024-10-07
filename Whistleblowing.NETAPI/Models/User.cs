using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Whistleblowing.NETAPI.Models
{
	public class User : IdentityUser
	{

		[Key]
		[Column("user_id")]
		public int Id { get; set; }

		[Column("nome")]
		public string Nome { get; set; }

		[Column("cognome")]
		public string Cognome { get; set; }

		[Column("codiceFiscale")]
		public string CodiceFiscale { get; set; }

		[Column("email")]
		public string Email { get; set; }

		[Column("password")]
		public string Password { get; set; }

		[Column("azienda")]
		public string Azienda { get; set; }

		[Column("posizione")]
		public string Posizione { get; set; }

		[Column("telefono")]
		public string Telefono { get; set; }

		[Column("data_nascita")]
		public DateTime DataNascita { get; set; }

		[Column("luogo_nascita")]
		public string LuogoNascita { get; set; }

		[Column("provincia")]
		public string Provincia { get; set; }

		//proprietà per il dto di registrazione fino a qui ^^

		[Column("isDeleted")]
		public Boolean IsDeleted { get; set; }

		[Column("isLoggedIn")]
		public Boolean IsLoggedIn { get; set; }

		[Column("HasChangedPassword")]
		public bool HasChangedPassword { get; set; }

		[Column("isExternal")]
		public Boolean IsExternal { get; set; }

		[Column("RuoloId")]
		public Ruolo? Ruolo { get; set; }

	}
}
