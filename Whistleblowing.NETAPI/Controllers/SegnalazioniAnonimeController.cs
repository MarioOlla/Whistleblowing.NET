using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whistleblowing.NETAPI.Data;
using Whistleblowing.NETAPI.DTO;
using Whistleblowing.NETAPI.Models;
using Whistleblowing.NETAPI.Models.view;
using Status = Whistleblowing.NETAPI.Models.Status;

namespace Whistleblowing.NETAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SegnalazioniAnonimeController : ControllerBase
	{


		//aggiungo il Db_Context
		private readonly WhistleBlowingContext _context;

		public SegnalazioniAnonimeController(WhistleBlowingContext context)
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
		[HttpGet("GetAllSegnalazioniAnonime")]
		public async Task<IActionResult> GetAllSegnalazioniAnonime([FromQuery] int userid, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			// Trovo l'utente corrente con il ruolo
			var user = _context.User.Include(u => u.Ruolo).FirstOrDefault(u => u.Id == userid);

			// Se l'utente non esiste, ritorno un errore
			if (user == null)
			{
				return NotFound("Utente non trovato");
			}

			// Verifico se l'utente è un operatore (codice ruolo == 2)
			var isOperatore = user.Ruolo?.codice == 2;

			// Se l'utente non è operatore, ritorno un errore di autorizzazione
			if (!isOperatore)
			{
				return Forbid("Accesso negato. Solo gli operatori possono visualizzare le segnalazioni anonime.");
			}

			// Query per ottenere tutte le segnalazioni anonime
			IQueryable<SegnalazioneAnonimaView> segnalazioniQuery = _context.SegnalazioneAnonimaViews;

			// Applico la paginazione
			segnalazioniQuery = segnalazioniQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize);

			// Ottengo la lista di segnalazioni
			List<SegnalazioneAnonimaView> segnalazioniAnonime = await segnalazioniQuery.ToListAsync();

			// Conteggio totale delle segnalazioni anonime
			var totalRecords = await _context.SegnalazioneAnonimaViews.CountAsync();

			// Preparo il risultato paginato
			var paginatedResult = new
			{
				TotalRecords = totalRecords,
				PageNumber = pageNumber,
				PageSize = pageSize,
				Data = segnalazioniAnonime
			};

			// Ritorno il risultato paginato
			return Ok(paginatedResult);
		}











		/// <summary>
		/// API per inserimento di una segnalazione Anonima
		/// </summary>
		/// <param name="segnalazioneRegularDTOInserimento">segnalazione da inserire</param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> PostSegnalazioneRegular(SegnalazioneAnonimaDTO segnalazioneAnonimaDTO)
		{
			//se la segnalazione è null torno errore
			if (segnalazioneAnonimaDTO == null)
			{
				return BadRequest();
			}

			//se il context non trova la tabella ritorno errore
			if (_context.SegnalazioneAnonymous == null)
			{
				return BadRequest();
			}

			// *** Creo il mio oggetto segnalazione per effettuare l' inserimento *** //
			var _segnalazioneAnonima = new SegnalazioneAnonymous()
			{
				FattoRiferitoA = segnalazioneAnonimaDTO.FattoRiferitoA,
				DataEvento = segnalazioneAnonimaDTO.DataEvento,
				LuogoEvento = segnalazioneAnonimaDTO.LuogoEvento,
				SoggettoColpevole = segnalazioneAnonimaDTO.SoggettoColpevole,
				AreaAziendale = segnalazioneAnonimaDTO.AreaAziendale,
				SoggettiPrivatiCoinvolti =	segnalazioneAnonimaDTO.SoggettiPrivatiCoinvolti,
				ImpreseCoinvolte = segnalazioneAnonimaDTO.ImpreseCoinvolte,
				PubbliciUfficialiPaCoinvolti = segnalazioneAnonimaDTO.PubbliciUfficialiPaCoinvolti,
				ModalitaConoscenzaFatto = segnalazioneAnonimaDTO.ModalitaConoscenzaFatto,
				SoggettiReferentiFatto = segnalazioneAnonimaDTO.SoggettiReferentiFatto,
				AmmontarePagamentoOAltraUtilita = segnalazioneAnonimaDTO.AmmontarePagamentoOAltraUtilita,
				CircostanzeViolenzaMinaccia = segnalazioneAnonimaDTO.CircostanzeViolenzaMinaccia,
				DescrizioneFatto = segnalazioneAnonimaDTO.DescrizioneFatto,
				MotivazioneFattoIllecito = segnalazioneAnonimaDTO.MotivazioneFattoIllecito,
				Note = segnalazioneAnonimaDTO.Note,
				// Imposto lo status su "APERTO" all'inserimento
				status = Status.APERTO,
			};

			//effettuo l' inserimento della segnalazione
			_context.SegnalazioneAnonymous.Add(_segnalazioneAnonima);

			//salvo le modifiche del context
			await _context.SaveChangesAsync();

			return Ok(_segnalazioneAnonima);

		}
	}
}
