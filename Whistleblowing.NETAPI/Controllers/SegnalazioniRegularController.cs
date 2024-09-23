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
		/// Endpoint che serve per ottenere tutte le segnalazioni fitrate per Ruolo User
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="pageNumber"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet("GetAllSegnalazioniRegular")]
		public async Task<IActionResult> GetAllSegnalazioniRegular([FromQuery] int userid, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			// per prima cosa trovo l'utente corrente
			var user = _context.User.Include(u => u.Ruolo).FirstOrDefault(u => u.Id == userid);

			// se non trovo l'utente torno errore
			if (user == null)
			{
				return NotFound("Utente non trovato");
			}

			var isOperatore = user.Ruolo?.codice == 2;

			// se l'utente che trovo è un operatore restituisco tutte le segnalazioni, altrimenti filtro per userId
			IQueryable<SegnalazioneRegularView> segnalazioniQuery = isOperatore
				? _context.SegnalazioneRegularViews
				: _context.SegnalazioneRegularViews.Where(s => s.UserId == userid);

			// Applico la paginazione
			segnalazioniQuery = segnalazioniQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize);

			// Per avere risposta converto in lista
			List<SegnalazioneRegularView> segnalazioniRegolari = await segnalazioniQuery.ToListAsync();

			// Restituisco anche informazioni sulla paginazione, se necessario
			var totalRecords = await _context.SegnalazioneRegularViews.CountAsync();
			var paginatedResult = new
			{
				TotalRecords = totalRecords,
				PageNumber = pageNumber,
				PageSize = pageSize,
				Data = segnalazioniRegolari
			};

			// infine ritorno i dati paginati
			return Ok(paginatedResult);
		}



	}
}
