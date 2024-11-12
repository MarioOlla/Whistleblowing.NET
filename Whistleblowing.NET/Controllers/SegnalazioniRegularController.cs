using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Whistleblowing.NET.Models;
using Whistleblowing.NET.Models.DTO;

namespace Whistleblowing.NET.Controllers
{
	public class SegnalazioniRegularController : Controller
	{
		private readonly HttpClient _client;

		private Uri baseAddress = new Uri("https://localhost:44300/api");

		private readonly IHttpContextAccessor _contextAccessor;

		public SegnalazioniRegularController(IHttpContextAccessor _contextAccs)
		{
			// *** Definisco il mio client *** //
			_client = new HttpClient();
			_client.BaseAddress = baseAddress;
			_client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			//creo una varibiale di sessione e la forzo a valore di id 1 per riuscire ad ottenere i dati
			_contextAccessor = _contextAccs;
		}
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var model = new PaginatedSegnalazioniRegularViewModel();

            try
            {
                var response = await _client.GetAsync($"{baseAddress}/SegnalazioniRegular/GetAllSegnalazioniRegularTotali?pageNumber={pageNumber}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                {
                    // Leggi il contenuto JSON come stringa e stampa nei log
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Risposta JSON ricevuta:");
                    Console.WriteLine(jsonResponse);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                    };

                    // Deserializza la risposta usando la classe wrapper
                    var wrapper = System.Text.Json.JsonSerializer.Deserialize<Wrapper>(jsonResponse);

                    if (wrapper != null && wrapper.Values != null)
                    {
                        model.SegnalazioniRegulars = wrapper.Values;
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Errore nel recupero delle segnalazioni.";
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Errore di rete: {ex.Message}";
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Errore nella deserializzazione della risposta JSON:");
                Console.WriteLine(ex.Message);
                ViewBag.ErrorMessage = "Errore nella deserializzazione dei dati.";
            }

            return View(model); // Passa il modello alla vista
        }




        /// <summary>
        /// Invia una segnalazione regolare
        /// </summary>
        /// <param name="segnalazioneRegular">Oggetto contenente i dati della segnalazione.</param>
        /// <returns>Risultato dell'operazione di invio.</returns>
        [HttpPost]
		public async Task<IActionResult> InviaSegnalazioneRegular([FromBody] SegnalazioneRegular segnalazioneRegular)
		{
			// Controlla se l'oggetto è valido
			if (segnalazioneRegular == null)
			{
				return BadRequest("Dati non validi.");
			}

			try
			{
				//invio la segnalazione all'API
				var response = await _client.PostAsJsonAsync("SegnalazioniRegular/PostSegnalazioneRegular", segnalazioneRegular);

				// controllo la risposta dell'API
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<SegnalazioneRegular>();
					return Ok(result);
				}
				else
				{
					var errorMessage = await response.Content.ReadAsStringAsync();
					return StatusCode((int)response.StatusCode, errorMessage);
				}
			}
			catch (HttpRequestException ex)
			{
				return StatusCode(500, $"Errore nella comunicazione con l'API: {ex.Message}");
			}
		}

        /// <summary>
        /// Ottiene tutte le segnalazioni indipendentemente dall'utente.
        /// </summary>
        /// <param name="pageNumber">Numero della pagina da visualizzare</param>
        /// <param name="pageSize">Numero di elementi per pagina</param>
        /// <returns>Risultato paginato con tutte le segnalazioni</returns>
        [HttpGet("GetAllSegnalazioniRegularTotali")]
        public async Task<IActionResult> GetAllSegnalazioniRegularTotali(int pageNumber = 1, int pageSize = 10)
        {
            try

            {

                // Chiamata all'endpoint del backend
                var response = await _client.GetAsync($"{baseAddress}/SegnalazioniRegular/GetAllSegnalazioniRegularTotali?pageNumber={pageNumber}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                {
                    // Leggi i dati della risposta
                    var data = await response.Content.ReadFromJsonAsync<dynamic>(); // Usa dynamic per il debug
                    if (data != null)
                    {
                        return Ok(data); // Ritorna i dati ricevuti
                    }
                }

                // Se la risposta non è andata a buon fine
                return StatusCode((int)response.StatusCode, "Errore nel recupero delle segnalazioni.");
            }
            catch (HttpRequestException ex)
            {
                // Gestisci le eccezioni di rete
                return StatusCode(500, $"Errore di rete: {ex.Message}");
            }
        }

        /// <summary>
        /// Ottieni una segnalazione regolare basata sul suo ID.
        /// </summary>
        /// <param name="segnalazioneRegularId">L'ID della segnalazione regolare</param>
        /// <returns>Ritorna la vista con i dettagli della segnalazione</returns>
        [HttpGet]
        public async Task<IActionResult> GetSegnalazioneRegularById(int Id)
        {
            try
            {
                // Chiamata all'API backend per ottenere la segnalazione tramite il suo ID
                var response = await _client.GetAsync($"{baseAddress}/SegnalazioniRegular/getSegnalazioneRegularById/{Id}");

                if (response.IsSuccessStatusCode)
                {
                    // Deserializza la risposta JSON in un oggetto SegnalazioneRegularView
                    var segnalazione = await response.Content.ReadFromJsonAsync<SegnalazioneRegularView>();

                    if (segnalazione != null)
                    {
                        // Ritorna la vista con i dettagli della segnalazione
                        return View("DettaglioSegnalazioneRegular", segnalazione);
                    }
                }

                // Gestione dei casi in cui la segnalazione non è stata trovata
                ViewBag.ErrorMessage = "Segnalazione non trovata.";
                return View("Errore");
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Errore di rete: {ex.Message}";
                return View("Errore");
            }
        }

        /// <summary>
        /// Ottieni una segnalazione regolare basata sul suo ID.
        /// </summary>
        /// <param name="segnalazioneRegularId">L'ID della segnalazione regolare</param>
        /// <returns>Ritorna la vista con i dettagli della segnalazione</returns>
        [HttpGet]
        public async Task<IActionResult> EditSegnalazioneRegularById(int Id)
        {
            try
            {
                // Chiamata all'API backend per ottenere la segnalazione tramite il suo ID
                var response = await _client.GetAsync($"{baseAddress}/SegnalazioniRegular/getSegnalazioneRegularById/{Id}");

                if (response.IsSuccessStatusCode)
                {
                    // Deserializza la risposta JSON in un oggetto SegnalazioneRegularView
                    var segnalazione = await response.Content.ReadFromJsonAsync<SegnalazioneRegularView>();

                    if (segnalazione != null)
                    {
                        // Ritorna la vista con i dettagli della segnalazione
                        return View("ModificaSegnalazioneRegular", segnalazione);
                    }
                }

                // Gestione dei casi in cui la segnalazione non è stata trovata
                ViewBag.ErrorMessage = "Segnalazione non trovata.";
                return View("Errore");
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Errore di rete: {ex.Message}";
                return View("Errore");
            }
        }

        /// <summary>
        /// Metodo per modificare una segnalazione Regular tramite chiamata API
        /// </summary>
        /// <param name="segnalazione">Oggetto SegnalazioneRegularDTOInserimento con i nuovi dati</param>
        /// <returns>ActionResult</returns>
        [HttpPut]
        public async Task<IActionResult> PutSegnalazioneRegular(SegnalazioneRegularDTOInserimento segnalazione)
        {
            // Recupera l'ID utente dalla sessione
            int? userid = _contextAccessor.HttpContext?.Session.GetInt32("UserId");

            // Se l'utente non è presente nella sessione, forziamo l'ID a 1 per testing
            if (userid == null)
            {
                userid = 1; // Forza l'ID per testing
                _contextAccessor.HttpContext?.Session.SetInt32("UserId", (int)userid);
            }

            // Se la segnalazione è nulla, restituisci un errore
            if (segnalazione == null)
            {
                return BadRequest("La segnalazione non può essere nulla");
            }

            // Definisci l'URL dell'endpoint dell'API
            string url = $"/Segnalazione/PutSegnalazioneRegular?userid={userid}";

            try
            {
                // Effettua la chiamata PUT all'API
                var response = await _client.PutAsJsonAsync(url, segnalazione);

                // Verifica la risposta del server
                if (response.IsSuccessStatusCode)
                {
                    // Se la modifica è andata a buon fine, reindirizza alla pagina Index
                    return RedirectToAction("Index", "SegnalazioniRegular");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    // Se l'accesso è negato, restituisci un errore 403
                    return Forbid("Accesso negato: solo gli utenti con codice OPERATORE possono modificare le segnalazioni!");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Se l'utente non è stato trovato, restituisci un errore 404
                    return NotFound("Utente non trovato");
                }
                else
                {
                    // Gestione di altri tipi di errore
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                // Gestione degli errori di rete
                return StatusCode(500, $"Errore durante la chiamata all'API: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Gestione di altri errori generali
                return StatusCode(500, $"Errore imprevisto: {ex.Message}");
            }
        }

    }
}
    






	

