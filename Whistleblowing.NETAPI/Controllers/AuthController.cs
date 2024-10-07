using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Text;
using Whistleblowing.NETAPI.Data;
using Whistleblowing.NETAPI.DTO;
using Whistleblowing.NETAPI.Models;
using Whistleblowing.NETAPI.Service;

namespace Whistleblowing.NETAPI.Controllers
{
	[Route("api/[controller]/[action]")]  // definisco la route per chiamata della richiesta API
	[ApiController]
	public class AuthController : ControllerBase
	{

		public readonly WhistleBlowingContext _context;
		private readonly IEmailService _emailService;

		private readonly Random _random;


		public AuthController(WhistleBlowingContext context, IEmailService emailService)
		{
			_emailService = emailService;
			_random = new Random();
			_context = context;
		}

		/// <summary>
		/// API per registrazione di un utente
		/// </summary>
		/// <param name="user">utente effettua il register</param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> Register(UtenteRegisterDTO utenteRegister)
		{
			if (ModelState.IsValid)
			{
				// Controllo se l'email è già in uso
				if (_context.User.Any(u => u.Email == utenteRegister.Email))
				{
					ModelState.AddModelError("Email", "L'email inserita è già in uso.");
					return BadRequest(ModelState);
				}

				// Codifica la password in Base64 (valuta l'uso di un hash più sicuro)
				var password = Convert.ToBase64String(Encoding.UTF8.GetBytes(utenteRegister.Password));

				// Crea un nuovo utente
				var newUser = new User
				{
					Nome = utenteRegister.Nome,
					Cognome = utenteRegister.Cognome,
					Email = utenteRegister.Email,
					Password = password, // Salva la password codificata
					Azienda = utenteRegister.Azienda,
					Posizione = utenteRegister.Posizione,
					Telefono = utenteRegister.Telefono,
					DataNascita = utenteRegister.DataNascita,
					LuogoNascita = utenteRegister.LuogoNascita,
					Provincia = utenteRegister.Provincia,
					CodiceFiscale = utenteRegister.CodiceFiscale,
					HasChangedPassword = utenteRegister.HasChangedPassword = true,

					Ruolo = await _context.Ruolo.FirstOrDefaultAsync(r => r.descrizione == "Utente") // Imposta ruolo utente
				};

				// Aggiungi il nuovo utente al contesto
				_context.User.Add(newUser);
				await _context.SaveChangesAsync();

				return Ok(newUser); // Restituisci l'utente appena creato
			}

			return BadRequest(ModelState);
		}


		/// <summary>
		/// API per il login di un utente
		/// </summary>
		/// <param name="user">utente che effettua il login</param>
		/// <returns></returns>
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login([Bind("Email,Password,IsLoggedIn")] UtenteLoginDTO utenteLogin)
		{
			if (ModelState.IsValid)
			{
				var existingUser = await _context.User
				.Include(u => u.Ruolo) // Forza Entity Framework a caricare il ruolo
				.FirstOrDefaultAsync(u => u.Email == utenteLogin.Email);

				if (existingUser == null)
				{
					ModelState.AddModelError("Email", "L'email non è registrata.");
					return BadRequest(ModelState);
				}

				var passwordCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(utenteLogin.Password));

				if (existingUser.Password != passwordCode)
				{
					ModelState.AddModelError("Password", "La password è errata.");
					return BadRequest(ModelState);
				}

				// Creo l'identità dell'utente che sarà salvata nel cookie
				var claims = new[]
				{
					new Claim(ClaimTypes.Email, existingUser.Email),
					new Claim(ClaimTypes.Role, existingUser.Ruolo.Id.ToString()),
					new Claim(ClaimTypes.Name, existingUser.Nome),
					new Claim(ClaimTypes.Surname, existingUser.Cognome),
					new Claim("UserId", existingUser.Id.ToString()),
					new Claim("Ruolo", existingUser.Ruolo.Id.ToString()),
				};

				var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

				var authProperties = new AuthenticationProperties
				{
					IsPersistent = utenteLogin.IsLoggedIn
				};

				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
					new ClaimsPrincipal(identity), authProperties);

				Console.WriteLine($"L'utente con email {existingUser.Email} ha effettuato il login con successo.");


				return Ok(existingUser);
			}

			return BadRequest(ModelState);
		}



		/// <summary>
		/// metodo che permette di effettuare il Logout dell' utente
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> Logout()
		{


			//Eseguo il logout dell' utente
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


			//Reindirizzo l' utente alla pagina di login
			return Ok(new { message = "Logged out succesfully" });

		}


		/// <summary>
		/// metodo che genera una password casuale in caso di password dimenticata
		/// </summary>
		/// <returns></returns>
		private string GenerateRandomPassword()
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var random = new Random();
			return new string(Enumerable.Repeat(chars, 8)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}

		/// <summary>
		/// metodo che assegna una nuova password all' utente
		/// </summary>
		/// <param name="emailRequest"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> ForgotPassword([FromBody] EmailRequest emailRequest)
		{
			if (string.IsNullOrEmpty(emailRequest.Email))
			{
				return BadRequest("Email non valida.");
			}

			var user = await _context.User.FirstOrDefaultAsync(u => u.Email == emailRequest.Email);
			if (user == null)
			{
				return NotFound("Utente non trovato.");
			}

			string newPassword = GenerateRandomPassword();
			user.HasChangedPassword = false;
			user.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(newPassword));
			_context.User.Update(user);
			await _context.SaveChangesAsync();

			string subject = "Nuova password per il ripristino dell'account";
			string message = $"La tua nuova password è: {newPassword}. Ti consigliamo di cambiarla al più presto.";
			await _emailService.SendEmailAsync(emailRequest.Email, subject, message);

			return Ok();
		}


		/// <summary>
		/// metodo che utilizzo per cambiare la password dopo aver ottenuto quella temporanea
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
		{
			// Controllo se la richiesta è valida
			if (string.IsNullOrEmpty(request.Mail) || string.IsNullOrEmpty(request.NewPassword))
			{
				return BadRequest("Impossibile cambiare la password.");
			}

			// Cerco l'utente nel database con l'email fornita
			var user = await _context.User.FirstOrDefaultAsync(u => u.Email == request.Mail);
			if (user == null)
			{
				return NotFound("Utente non trovato.");
			}

			// Aggiorno la password nel database
			user.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.NewPassword));
			user.HasChangedPassword = true;
			_context.User.Update(user);
			await _context.SaveChangesAsync();

			// Restituisco un'esito positivo
			return Ok();
		}





		





	}
}
