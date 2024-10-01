using System.ComponentModel.DataAnnotations.Schema;

namespace Whistleblowing.NETAPI.DTO
{
	/// <summary>
	/// DTO UtenteLogin per accesso utente
	/// <see cref="https://learn.microsoft.com/it-it/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5"/>
	/// </summary>
	[Table("User")]
	public class UtenteLoginDTO
	{
		[Column("email")]
		public string Email { get; set; }

		[Column("password")]
		public string Password { get; set; }

		[Column("isLoggedIn")]
		public Boolean IsLoggedIn { get; set; }

	}
}
