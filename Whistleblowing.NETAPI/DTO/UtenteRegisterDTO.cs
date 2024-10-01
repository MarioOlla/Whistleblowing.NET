using System.ComponentModel.DataAnnotations.Schema;

namespace Whistleblowing.NETAPI.DTO
{
	/// <summary>
	/// DTO UtenteRegister per registrazione utente
	/// <see cref="https://learn.microsoft.com/it-it/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5"/>
	/// </summary>
	[Table("User")]
	public class UtenteRegisterDTO
	{
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
	}
}
