using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using Whistleblowing.NETAPI.Data;
using Whistleblowing.NETAPI.Models;

namespace Whistleblowing.NETAPI.Controllers
{
	[Route("api/[controller]/[action]")]  // definisco la route per chiamata della richiesta API
	[ApiController]
	public class AuthController : ControllerBase
	{

		public readonly WhistleBlowingContext _context;


		public AuthController(WhistleBlowingContext context)
		{

			_context = context;
		}


		[HttpPost]
		public async Task<IActionResult> Register(User user)
		{
			if (ModelState.IsValid)
			{
				// Controllo se l'email è già in uso
				if (_context.User.Any(u => u.Email == user.Email))
				{
					ModelState.AddModelError("Email", "L'email inserita è già in uso.");
					return BadRequest(ModelState);
				}

				// Codifica la password in Base64 (valuta l'uso di un hash più sicuro)
				var password = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Password));

				// Crea un nuovo utente
				var newUser = new User
				{
					Nome = user.Nome,
					Cognome = user.Cognome,
					Email = user.Email,
					Password = password, // Salva la password codificata
					Azienda = user.Azienda,
					Posizione = user.Posizione,
					Telefono = user.Telefono,
					DataNascita = user.DataNascita,
					LuogoNascita = user.LuogoNascita,
					Provincia = user.Provincia,
					CodiceFiscale = user.CodiceFiscale,
					IsDeleted = false, // Nuovi utenti non eliminati
					IsLoggedIn = false, // Nuovi utenti non loggati
					Ruolo = await _context.Ruolo.FirstOrDefaultAsync(r => r.descrizione == "Utente") // Imposta ruolo utente
				};

				// Aggiungi il nuovo utente al contesto
				_context.User.Add(newUser);
				await _context.SaveChangesAsync();

				return Ok(newUser); // Restituisci l'utente appena creato
			}

			return BadRequest(ModelState);
		}





	}
}
