using System.Collections.Generic;
using Whistleblowing.NETAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace Whistleblowing.NETAPI.Data
{
	public class WhistleBlowingContext : DbContext
	{
		public WhistleBlowingContext(DbContextOptions<WhistleBlowingContext> options)
		 : base(options)
		{
		}

		DbSet<Ruolo> Ruolo { get; set; } = default!;

		DbSet<User> User { get; set; } = default!;

		DbSet<SegnalazioneRegular> segnalazioneRegulars { get; set; } = default!;

		DbSet<SegnalazioneAnonymous> SegnalazioneAnonymous { get; set; } = default!;




	}
}
