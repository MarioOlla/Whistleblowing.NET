using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whistleblowing.NETAPI.Data;
using Whistleblowing.NETAPI.DTO;
using Whistleblowing.NETAPI.Models;
using Whistleblowing.NETAPI.Models.view;
using Status = Whistleblowing.NETAPI.Models.Status;
using System.Security.Claims;
using System.Security.Cryptography;
using Whistleblowing.NETAPI.Crypto;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Whistleblowing.NETAPI.Service;
namespace Whistleblowing.NETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
	public class SegnalazioniRegularController : ControllerBase
	{
		//aggiungo il Db_Context
		private readonly WhistleBlowingContext _context;

		//aggiungo il pdfService
		private readonly PdfService _pdfService;

		public SegnalazioniRegularController(WhistleBlowingContext context, PdfService pdfService)
		{
			_context = context;
			_pdfService = pdfService;
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

			var isOperatore = user.Ruolo.ToString().Equals("OPERATORE");

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
        /// Endpoint per ottenere tutte le segnalazioni regular dell'utente loggato utilizzando il token JWT
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMySegnalazioniRegular")]
        [Authorize]
        public async Task<IActionResult> GetMySegnalazioniRegular([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Ottieni l'ID utente dal token JWT
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Non autorizzato. ID utente non trovato.");
            }

            // Verifica che l'utente esista e non sia eliminato
            var userExists = await _context.User
                .AnyAsync(u => u.Id == userId && !u.IsDeleted);
            if (!userExists)
            {
                return NotFound("Utente non trovato o eliminato.");
            }

            // Filtra le segnalazioni per l'utente
            IQueryable<PaginatedSegnalazioniRegularViewUtente> segnalazioniQuery = _context.paginatedSegnalazioniRegularViewUtentes
                .Where(s => s.UserId == userId);

            // Ottieni il numero totale di record
            var totalRecords = await segnalazioniQuery.CountAsync();

            // Applica l'ordinamento e la paginazione
            segnalazioniQuery = segnalazioniQuery
                .OrderByDescending(s => s.DataEvento) // Ordina per DataEvento
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // Esegui la query per ottenere i dati paginati
            List<PaginatedSegnalazioniRegularViewUtente> segnalazioniRegolari = await segnalazioniQuery.ToListAsync();

            // Crea un oggetto per la risposta con paginazione
            var paginatedResult = new
            {
                TotalItems = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = segnalazioniRegolari
            };

            return Ok(paginatedResult);
        }



        [HttpGet("segnalazioneModifica/{id}")]
        public async Task<IActionResult> GetSegnalazioneModifica(int id)
        {
            var segnalazione = await _context.segnalazioneRegulars.FirstOrDefaultAsync(s => s.Id == id);

            if (segnalazione == null)
            {
                return NotFound();
            }

            // Mappatura dei campi dal modello dell'entità al DTO
            var dto = new SegnalazioneRegularDTOModifica
            {
                Id = segnalazione.Id,
                FattoRiferitoA = segnalazione.FattoRiferitoA ?? string.Empty, // Evita valori null
                DataEvento = segnalazione.DataEvento, // Assumendo che DataEvento sia già nullable in entrambi i modelli
                LuogoEvento = segnalazione.LuogoEvento ?? string.Empty,
                SoggettoColpevole = segnalazione.SoggettoColpevole ?? string.Empty,
                AreaAziendale = segnalazione.AreaAziendale ?? string.Empty,
                SoggettiPrivatiCoinvolti = segnalazione.SoggettiPrivatiCoinvolti ?? string.Empty,
                ImpreseCoinvolte = segnalazione.ImpreseCoinvolte ?? string.Empty,
                PubbliciUfficialiPaCoinvolti = segnalazione.PubbliciUfficialiPaCoinvolti ?? string.Empty,
                ModalitaConoscenzaFatto = segnalazione.ModalitaConoscenzaFatto ?? string.Empty,
                SoggettiReferentiFatto = segnalazione.SoggettiReferentiFatto ?? string.Empty,
                AmmontarePagamentoOAltraUtilita = segnalazione.AmmontarePagamentoOAltraUtilita ?? string.Empty,
                CircostanzeViolenzaMinaccia = segnalazione.CircostanzeViolenzaMinaccia ?? string.Empty,
                DescrizioneFatto = segnalazione.DescrizioneFatto ?? string.Empty,
                MotivazioneFattoIllecito = segnalazione.MotivazioneFattoIllecito ?? string.Empty,
                Note = segnalazione.Note ?? string.Empty,
                status = segnalazione.status
            };

            return Ok(dto);
        }





        /// <summary>
        /// Endpoint che serve per ottenere tutte le segnalazioni indipendentemente dall'utente
        /// </summary>
        /// <param name="pageNumber">Numero della pagina da visualizzare</param>
        /// <param name="pageSize">Numero di elementi per pagina</param>
        /// <returns>Risultato paginato con tutte le segnalazioni</returns>
        [HttpGet("GetAllSegnalazioniRegularTotali")]
        public async Task<IActionResult> GetAllSegnalazioniRegularTotali(int userId, [FromQuery] int pageNumber = 1,
            [FromQuery] int? segnalazioneId = null,

            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? searchDate = null)
        {
            var user = await _context.User.FindAsync(userId);

            // Recupera il numero totale di segnalazioni
            var totalRecords = await _context.paginatedSegnalazioniRegularViewModels.Where(s => s.IsDeleted == false).CountAsync();
            IQueryable<PaginatedSegnalazioniAnonimeViewModel> query = _context.paginatedSegnalazioniAnonimeViewModels
                .Where(s => s.IsDeleted == false);


            // Filtro per stato
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse(typeof(Status), status, true, out var statusEnum))
                {
                    var statusValue = (Status)statusEnum;
                    query = query.Where(s => s.status == statusValue);
                }
                else
                {
                    return BadRequest(new { message = "Stato non valido. Usa APERTO, LAVORAZIONE o CHIUSO." });
                }
            }

            // Filtro per data (cerca segnalazioni con la stessa data)
            if (searchDate.HasValue)
            {
                query = query.Where(s => s.DataEvento == searchDate.Value.Date);
            }

            if (segnalazioneId != null)
            {
                query = query.Where(s => s.segnalazione_anonima_id == segnalazioneId.Value);
            }


            // Applica la paginazione
            var segnalazioniQuery = _context.paginatedSegnalazioniRegularViewModels
                .Where(s => s.IsDeleted == false)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var segnalazioniRegolari = await segnalazioniQuery.ToListAsync();

            // Crea il risultato paginato con i metadati
            var paginatedResult = new
            {
                TotalItems = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = segnalazioniRegolari
            };

            return Ok(paginatedResult);
        }




        /// <summary>
        /// Metodo che utilizzo per ottenere una segnalazione Regolare in base al suo id e l' id utente
        /// </summary>
        /// <param name="segnalazioneRegularId"></param>
        /// <returns></returns>
        [HttpGet("getSegnalazioneRegularById/{Id}")]
		public async Task<ActionResult<SegnalazioneRegularView>> getSegnalazioneRegularById(int Id)
		{
            // cerco la segnalazione con il suo id
            var segnalazione = await _context.SegnalazioneRegularViews.Where(s => s.Id == Id).Include(u => u.user).SingleOrDefaultAsync();

            //se invece non trovo la segnalazione restituisco un NotFound
            if (segnalazione == null)
			{
				return NotFound(new { message = "segnalazione non trovata!" });
			}

			//se tutto è ok torno la segnalazione
			return Ok(segnalazione);


        }


        /// <returns></returns>
        [HttpGet("getDeletedSegnalazioneRegularById/{Id}")]
        public async Task<ActionResult<SegnalazioneRegularView>> getDeletedSegnalazioneRegularById(int Id)
        {
            // cerco la segnalazione con il suo id
            var segnalazione = await _context.SegnalazioneRegularViews.SingleOrDefaultAsync(s => s.Id == Id);

            //se invece non trovo la segnalazione restituisco un NotFound
            if (segnalazione == null)
            {
                return NotFound(new { message = "segnalazione non trovata!" });
            }

            //se tutto è ok torno la segnalazione
            return Ok(segnalazione);


        }


        [HttpGet("SegnalazioneRegularPdfById")]
        [Authorize]
        public async Task<IActionResult> GetSegnalazioneRegularPdfById(int segnalazioneRegularId)
        {
            // Recupero la segnalazione dal database
            var segnalazione = await _context.SegnalazioneRegularViews
                .FirstOrDefaultAsync(s => s.Id == segnalazioneRegularId);

            // Controllo se la segnalazione è a null
            if (segnalazione == null)
            {
                return NotFound(new { message = "Segnalazione non trovata!" });
            }

            // Verifica se l'utente associato esiste
            var user = await _context.User.FindAsync(segnalazione.UserId);
            if (user == null)
            {
                return NotFound(new { message = "Utente associato non trovato!" });
            }

            // Genero il PDF utilizzando pdfService
            var pdfBytes = _pdfService.GenerateSegnalazioneRegularPdf(segnalazione, user.Nome, user.Cognome);

            // Restituisce il PDF come File
            return File(pdfBytes, "application/pdf", $"segnalazione_{segnalazioneRegularId}.pdf");
        }


        /// <summary>
        /// API per inserimento di una segnalazione Regular
        /// </summary>
        /// <param name="segnalazioneRegularDTOInserimento">segnalazione da inserire</param>
        /// <returns></returns>
        [HttpPost("PostSegnalazioneRegular")]
        [Authorize] 
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

			//estraggo l' ID dell' utente autenticato dalle claim del token JWT
			var userIdString = User.FindFirst("UserId")?.Value;

			//controllo per vedere se le claims non abbiano problemi
			if (string.IsNullOrEmpty(userIdString))
			{
				return Unauthorized("Non è stato possibile identificare l'utente dalle claims");
			}

			//Converto l' ID Utente in un intero
			if(!int.TryParse(userIdString, out int userId))
			{
				return BadRequest("ID Utente non valido");
			}

			//Recupero l' utente dal database usando l' ID estratto dal token JWT
			var user = await _context.User.FindAsync(userId);

			if(user == null)
			{
				return NotFound("Utente non trovato");
			}

			//stampa per visualizzare se i dati arrivano correttamente
			Console.WriteLine(user.Id.ToString(), user.Nome, user.Cognome, user.Email);

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
                UserId = userId,
                IsDeleted = segnalazioneRegularDTOInserimento.IsDeleted == true,
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
        [HttpPost("PutSegnalazioneRegular")]
        [Authorize]
        public async Task<ActionResult> PutSegnalazioneRegular([FromQuery] int userid, SegnalazioneRegularDTOModifica segnalazione)
        {
            // Recupera l'ID utente dal JWT
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            // Verifica che il claim esista e sia valido
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                Console.WriteLine("ciao jwt " + userIdClaim);
                return Problem("Token JWT invalido o mancante.");
            }

            // Trova l'utente nel database
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId);


            //se l'utente è null, restituisco errore
            if (user == null)
            {
                return NotFound("Utente non trovato");
            }

            // Verifico se l'utente è un operatore (codice 2)
            var isOperatore = user.Ruolo == Ruolo.OPERATORE;

            //se l' utente non è un OPERATORE, ritorno un errore di accesso negato
            if (!isOperatore)
            {
                return Forbid("Accesso negato: solo gli utenti con codice OPERATORE possono modificare le segnalazioni!");
            }

            // Trovo la segnalazione da modificare
            var segnalazioneEdit = await _context.segnalazioneRegulars.FindAsync(segnalazione.Id);

            //se la segnalazione è null restituisco errore
            if (segnalazioneEdit == null)
            {
                return Problem("Non è stata trovata alcuna segnalazione con l' id fornito!");
            }

            //aggiorno i campi della segnalazione con i campi forniti dal DTO 
            if (!segnalazione.FattoRiferitoA.IsNullOrEmpty()) segnalazioneEdit.FattoRiferitoA = segnalazione.FattoRiferitoA;
            if (segnalazione.DataEvento.HasValue) segnalazioneEdit.DataEvento = segnalazione.DataEvento.Value;
            if (!segnalazione.LuogoEvento.IsNullOrEmpty()) segnalazioneEdit.LuogoEvento = segnalazione.LuogoEvento;
            if (!segnalazione.SoggettoColpevole.IsNullOrEmpty()) segnalazioneEdit.SoggettoColpevole = segnalazione.SoggettoColpevole;
            if (!segnalazione.AreaAziendale.IsNullOrEmpty()) segnalazioneEdit.AreaAziendale = segnalazione.AreaAziendale;
            if (!segnalazione.SoggettiPrivatiCoinvolti.IsNullOrEmpty()) segnalazioneEdit.SoggettiPrivatiCoinvolti = segnalazione.SoggettiPrivatiCoinvolti;
            if (!segnalazione.ImpreseCoinvolte.IsNullOrEmpty()) segnalazioneEdit.ImpreseCoinvolte = segnalazione.ImpreseCoinvolte;
            if (!segnalazione.PubbliciUfficialiPaCoinvolti.IsNullOrEmpty()) segnalazioneEdit.PubbliciUfficialiPaCoinvolti = segnalazione.PubbliciUfficialiPaCoinvolti;
            if (!segnalazione.ModalitaConoscenzaFatto.IsNullOrEmpty()) segnalazioneEdit.ModalitaConoscenzaFatto = segnalazione.ModalitaConoscenzaFatto;
            if (!segnalazione.SoggettiReferentiFatto.IsNullOrEmpty()) segnalazioneEdit.SoggettiReferentiFatto = segnalazione.SoggettiReferentiFatto;
            if (!segnalazione.AmmontarePagamentoOAltraUtilita.IsNullOrEmpty()) segnalazioneEdit.AmmontarePagamentoOAltraUtilita = segnalazione.AmmontarePagamentoOAltraUtilita;
            if (!segnalazione.CircostanzeViolenzaMinaccia.IsNullOrEmpty()) segnalazioneEdit.CircostanzeViolenzaMinaccia = segnalazione.CircostanzeViolenzaMinaccia;
            if (!segnalazione.DescrizioneFatto.IsNullOrEmpty()) segnalazioneEdit.DescrizioneFatto = segnalazione.DescrizioneFatto;
            if (!segnalazione.MotivazioneFattoIllecito.IsNullOrEmpty()) segnalazioneEdit.MotivazioneFattoIllecito = segnalazione.MotivazioneFattoIllecito;
            if (segnalazione.status.HasValue) segnalazioneEdit.status = segnalazione.status.Value;
            if (!segnalazione.Note.IsNullOrEmpty()) segnalazioneEdit.Note = segnalazione.Note;

            //eseguo la modifica dei dati
            _context.segnalazioneRegulars.Update(segnalazioneEdit);

            //salvo le modifiche effettuate
            await _context.SaveChangesAsync();

            return Ok(segnalazioneEdit);
        }



        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteSegnalazioneRegular(int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Problem("Token JWT invalido o mancante.");
            }

            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || user.Ruolo != Ruolo.OPERATORE)
            {
                return Forbid("Accesso negato: solo gli utenti con codice OPERATORE possono modificare le segnalazioni!");
            }

            var segnalazione = await _context.segnalazioneRegulars.FindAsync(id);
            if (segnalazione == null)
            {
                return NotFound("Segnalazione non trovata.");
            }

            segnalazione.IsDeleted = true;
            segnalazione.status = Status.CHIUSO;
            await _context.SaveChangesAsync();

            return Ok();
        }















    }
}
