using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whistleblowing.NETAPI.Data;
using Whistleblowing.NETAPI.DTO;
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



		/// <summary>
		/// Metodo che utilizzo per ottenere una segnalazione Regolare in base al suo id e l' id utente
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="segnalazioneRegularId"></param>
		/// <returns></returns>
		[HttpGet("getSegnalazioneRegularById")]
		public async Task<ActionResult<SegnalazioneRegularView>> getSegnalazioneRegularById(int userId, int segnalazioneRegularId)
		{
			//Per prima cosa verifico che l' utente esista
			var utenteEsistente = await _context.User.AnyAsync(u => u.Id == userId && !u.IsDeleted);

			//se non esiste torno NotFound
			if (!utenteEsistente)
			{
				return NotFound(new { message = "utente non trovato!" });
			}

			//se quindi esiste cerco la segnalazione con il suo id e quello dell' utente specificato
			var segnalazione = await _context.SegnalazioneRegularViews.FirstOrDefaultAsync(s => s.UserId == userId && s.Id == segnalazioneRegularId);

			//se invece non trovo la segnalazione restituisco un NotFound
			if(segnalazione == null)
			{
				return NotFound(new { message = "segnalazione non trovata!" });
			}

			//se tutto è ok torno la segnalazione
			return Ok(segnalazione);


		}

		/// <summary>
		/// API per inserimento di una segnalazione Regular
		/// </summary>
		/// <param name="segnalazioneRegularDTOInserimento">segnalazione da inserire</param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> PostSegnalazioneRegular(SegnalazioneRegularDTOInserimento segnalazioneRegularDTOInserimento)
		{
			//se la segnalazione è null torno errore
			if(segnalazioneRegularDTOInserimento == null)
			{
				return BadRequest();
			}

			//se il context non trova la tabella ritorno errore
			if(_context.segnalazioneRegulars == null)
			{
				return BadRequest();
			}

			// *** Creo il mio oggetto segnalazione per effettuare l' inserimento *** //
			var _segnalazioneRegular = new SegnalazioneRegular()
			{
				FattoRiferitoA = segnalazioneRegularDTOInserimento.FattoRiferitoA,
				DataEvento = segnalazioneRegularDTOInserimento.DataEvento,
				LuogoEvento = segnalazioneRegularDTOInserimento.LuogoEvento,
				SoggettoColpevole = segnalazioneRegularDTOInserimento.SoggettoColpevole,
				AreaAziendale = segnalazioneRegularDTOInserimento.AreaAziendale,
				SoggettiPrivatiCoinvolti = segnalazioneRegularDTOInserimento.SoggettiPrivatiCoinvolti,
				ImpreseCoinvolte = segnalazioneRegularDTOInserimento.ImpreseCoinvolte,
				PubbliciUfficialiPaCoinvolti = segnalazioneRegularDTOInserimento.PubbliciUfficialiPaCoinvolti,
				ModalitaConoscenzaFatto = segnalazioneRegularDTOInserimento.ModalitaConoscenzaFatto,
				SoggettiReferentiFatto = segnalazioneRegularDTOInserimento.SoggettiReferentiFatto,
				AmmontarePagamentoOAltraUtilita = segnalazioneRegularDTOInserimento.AmmontarePagamentoOAltraUtilita,
				CircostanzeViolenzaMinaccia = segnalazioneRegularDTOInserimento.CircostanzeViolenzaMinaccia,
				DescrizioneFatto = segnalazioneRegularDTOInserimento.DescrizioneFatto,
				MotivazioneFattoIllecito = segnalazioneRegularDTOInserimento.MotivazioneFattoIllecito,
				Note = segnalazioneRegularDTOInserimento.Note,
				// Imposto lo status su "APERTO" all'inserimento
				status = SegnalazioneRegular.Status.APERTO,


			};

			//effettuo l' inserimento della segnalazione
			_context.segnalazioneRegulars.Add(_segnalazioneRegular);

			//salvo le modifiche del context
			await _context.SaveChangesAsync();

			return Ok(_segnalazioneRegular);

		}









	}
}
