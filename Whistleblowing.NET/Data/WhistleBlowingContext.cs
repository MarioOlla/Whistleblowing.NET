using Microsoft.EntityFrameworkCore;
using Whistleblowing.NET.Models;

namespace Whistleblowing.NET.Data
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
