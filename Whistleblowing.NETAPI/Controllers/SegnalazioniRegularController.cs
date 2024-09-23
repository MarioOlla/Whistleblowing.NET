using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whistleblowing.NETAPI.Data;
using Whistleblowing.NETAPI.Models;
using Whistleblowing.NETAPI.Models.view;

namespace Whistleblowing.NETAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SegnalazioniRegularController : ControllerBase
	{
		//aggiungo il Db_Context
		private readonly WhistleBlowingContext _context;

		public SegnalazioniRegularController(WhistleBlowingContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Endpoint che serve per ottenere tutte le segnalazioni fitrate per user:OPERATORE
		/// </summary>
		/// <param name="userid"></param>
		/// <returns></returns>
		[HttpGet("GetAllSegnalazioniRegular")]
		public async Task<IActionResult> GetAllSegnalazioniRegular([FromQuery] int userid)
		{
			// per prima cosa trovo l'utente corrente
			var user = _context.User.Include(u => u.Ruolo).FirstOrDefault(u => u.Id == userid);

			//se non trovo l' utente torno errore
			if(user == null)
			{
				return NotFound("Utente non trovato");
			}

			var isOperatore = false;

			var ruoloCodice = user.Ruolo?.codice;

			if(ruoloCodice == 2)
			{
				isOperatore = true;
			}
			else
			{
				isOperatore = false;
			}
			//se l'utente che trovo è un operatore restituisco tutte le segnalazioni
			IQueryable<SegnalazioneRegularView> segnalazioniQuery;
			if (isOperatore)
			{
				segnalazioniQuery = _context.SegnalazioneRegularViews;
			}
			else
			{
				segnalazioniQuery = _context.SegnalazioneRegularViews.Where(s => s.UserId == userid);
			}

			//per avere risposta converto in lista
			List<SegnalazioneRegularView> segnalazioniRegolari = await segnalazioniQuery.ToListAsync();

			//infine ritorno la lista
			return Ok(segnalazioniRegolari);



        }



	}
}
