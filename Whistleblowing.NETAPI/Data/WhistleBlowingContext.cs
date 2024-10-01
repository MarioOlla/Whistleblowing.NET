using System.Collections.Generic;
using Whistleblowing.NETAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Whistleblowing.NETAPI.Models.view;


namespace Whistleblowing.NETAPI.Data
{
	public class WhistleBlowingContext : DbContext
	{
		

		public WhistleBlowingContext(DbContextOptions<WhistleBlowingContext> options)
		 : base(options)
		{
		}

		public DbSet<Ruolo> Ruolo { get; set; } = default!;

		public DbSet<User> User { get; set; } = default!;

		public DbSet<SegnalazioneRegular> segnalazioneRegulars { get; set; } = default!;

		public DbSet<SegnalazioneAnonymous> SegnalazioneAnonymous { get; set; } = default!;

		public DbSet<SegnalazioneRegularView> SegnalazioneRegularViews { get; set; } = default!;

		public DbSet<CryptoKey> CryptoKey { get; set; } = default!;


		
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseLazyLoadingProxies(false); // Disabilita i proxy dinamici
		}


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<SegnalazioneRegularView>().HasNoKey().ToView("SegnalazioneRegularView");
		}

	}
}
