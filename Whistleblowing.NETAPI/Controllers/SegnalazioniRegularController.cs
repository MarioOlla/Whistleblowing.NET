using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Whistleblowing.NETAPI.Data;
using Whistleblowing.NETAPI.DTO;
using Whistleblowing.NETAPI.Models;
using Whistleblowing.NETAPI.Models.view;
using Status = Whistleblowing.NETAPI.Models.Status;

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
				status = Status.APERTO,


			};

			//effettuo l' inserimento della segnalazione
			_context.segnalazioneRegulars.Add(_segnalazioneRegular);

			//salvo le modifiche del context
			await _context.SaveChangesAsync();

			return Ok(_segnalazioneRegular);

		}


		/// <summary>
		/// API per modificare una segnalazione Regular, consentito solo agli utenti con ruolo OPERATORE
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="segnalazione"></param>
		/// <returns></returns>
		[HttpPut]
		public async Task<ActionResult> PutSegnalazioneRegular([FromQuery] int userid,SegnalazioneRegularDTOInserimento segnalazione)
		{
			//trovo l' utente corrente e ne controllo il ruolo
			var user = _context.User.Include(u=> u.Ruolo).FirstOrDefault(u=> u.Id == userid );


			//se l'utente è null, restituisco errore
			if (user == null)
			{
				return NotFound("Utente non trovato");
			}

			// Verifico se l'utente è un operatore (codice 2)
			var isOperatore = user.Ruolo?.codice == 2;

			//se l' utente non è un OPERATORE, ritorno un errore di accesso negato
			if(!isOperatore)
			{
				return Forbid("Accesso negato: solo gli utenti con codice OPERATORE possono modificare le segnalazioni!");
			}


			// Trovo la segnalazione da modificare
			var segnalazioneEdit = await _context.segnalazioneRegulars.FindAsync(segnalazione.Id);

			//se la segnalazione è null restituisco errore
			if(segnalazioneEdit == null)
			{
				return Problem("non è stata trovata alcuna segnalazione con l' id fornito!");
			}

			//aggiorno i campi della segnalazione con i campi forniti dal DTO 
			if(!segnalazione.FattoRiferitoA.IsNullOrEmpty()) segnalazioneEdit.FattoRiferitoA = segnalazione.FattoRiferitoA;
			if(segnalazione.DataEvento.HasValue) segnalazioneEdit.DataEvento = segnalazione.DataEvento.Value;
			if(!segnalazione.LuogoEvento.IsNullOrEmpty()) segnalazioneEdit.LuogoEvento = segnalazione.LuogoEvento;
			if (!segnalazione.SoggettoColpevole.IsNullOrEmpty()) segnalazioneEdit.SoggettoColpevole = segnalazione.SoggettoColpevole;
			if(!segnalazione.AreaAziendale.IsNullOrEmpty()) segnalazioneEdit.AreaAziendale = segnalazione.AreaAziendale;
			if (!segnalazione.SoggettiPrivatiCoinvolti.IsNullOrEmpty()) segnalazioneEdit.SoggettiPrivatiCoinvolti = segnalazione.SoggettiPrivatiCoinvolti;
			if (!segnalazione.ImpreseCoinvolte.IsNullOrEmpty()) segnalazioneEdit.ImpreseCoinvolte = segnalazione.ImpreseCoinvolte;
			if (!segnalazione.PubbliciUfficialiPaCoinvolti.IsNullOrEmpty()) segnalazioneEdit.PubbliciUfficialiPaCoinvolti = segnalazione.PubbliciUfficialiPaCoinvolti;
			if (!segnalazione.ModalitaConoscenzaFatto.IsNullOrEmpty()) segnalazioneEdit.ModalitaConoscenzaFatto = segnalazione.ModalitaConoscenzaFatto;
			if (!segnalazione.SoggettiReferentiFatto.IsNullOrEmpty()) segnalazioneEdit.SoggettiReferentiFatto = segnalazione.SoggettiReferentiFatto;
			if (!segnalazione.AmmontarePagamentoOAltraUtilita.IsNullOrEmpty()) segnalazioneEdit.AmmontarePagamentoOAltraUtilita = segnalazione.AmmontarePagamentoOAltraUtilita;
			if(!segnalazione.CircostanzeViolenzaMinaccia.IsNullOrEmpty()) segnalazioneEdit.CircostanzeViolenzaMinaccia = segnalazione.CircostanzeViolenzaMinaccia;
			if (!segnalazione.DescrizioneFatto.IsNullOrEmpty()) segnalazioneEdit.DescrizioneFatto = segnalazione.DescrizioneFatto;
			if(!segnalazione.MotivazioneFattoIllecito.IsNullOrEmpty()) segnalazioneEdit.MotivazioneFattoIllecito = segnalazione.MotivazioneFattoIllecito;
			if (!segnalazione.Note.IsNullOrEmpty()) segnalazioneEdit.Note = segnalazione.Note;

			//eseguo la modifica dei dati
			_context.segnalazioneRegulars.Update(segnalazioneEdit);

			//salvo le modifiche effettuate
			await _context.SaveChangesAsync();

			return Ok(segnalazioneEdit);


		}


		[HttpPut("{id}")]
		public async Task<ActionResult<SegnalazioneRegular>> DeleteSegnalazioneRegular( int id, [FromQuery] int userid)
		{

			//trovo l' utente corrente e ne controllo il ruolo
			var user = _context.User.Include(u => u.Ruolo).FirstOrDefault(u => u.Id == userid);


			//se l'utente è null, restituisco errore
			if (user == null)
			{
				return NotFound("Utente non trovato");
			}

			// Verifico se l'utente è un operatore (codice 2)
			var isOperatore = user.Ruolo?.codice == 2;

			//se l' utente non è un OPERATORE, ritorno un errore di accesso negato
			if (!isOperatore)
			{
				return Forbid("Accesso negato: solo gli utenti con codice OPERATORE possono modificare le segnalazioni!");
			}


			//se l' id segnalazione risulta null, oppure dal context risulta null o il suo id è inferiore a zero torno NotFound
			if (_context.segnalazioneRegulars == null || id == null || id < 0)
			{
				return NotFound();
			}

			//il context cerca a database la segnalazione da eliminare
			SegnalazioneRegular segnalazione = await _context.segnalazioneRegulars.FindAsync(id);

			//se la segnlazione è null torno errore
			if (segnalazione == null)
			{
				return NotFound();
			}

			//se tutto va bene imposto il booleano a true
			segnalazione.IsDeleted = true;

			//salvo le modifiche 
			await _context.SaveChangesAsync();

			//modifico la segnalazione
			_context.segnalazioneRegulars.Update(segnalazione);

			return Ok();


		}












	}
}
