using Microsoft.AspNetCore.Mvc;
using static Whistleblowing.NET.Controllers.SegnalazioniRegularController;
using System.Text.Json;
using Whistleblowing.NET.Models;
using System.Net.Http.Headers;

namespace Whistleblowing.NET.Controllers
{
	public class SegnalazioniAnonimeController : Controller
	{
        private readonly HttpClient _client;

        private Uri baseAddress = new Uri("https://localhost:44300/api");

        private readonly IHttpContextAccessor _contextAccessor;

        public SegnalazioniAnonimeController(IHttpContextAccessor _contextAccs)
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
            try
            {
                var response = await _client.GetAsync($"{baseAddress}/SegnalazioniAnonime/GetAllSegnalazioniAnonimeTotali?pageNumber={pageNumber}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserializza l'oggetto che contiene i dati e i metadati di paginazione
                    var result = System.Text.Json.JsonSerializer.Deserialize<PaginatedResponse<PaginatedSegnalazioniAnonimeViewModel>>(jsonResponse, options);

                    var viewModel = new PaginatedSegnalazioniAnonimeViewModel
                    {
                        SegnalazioniAnonymous = result.Data,
                        PageNumber = result.PageNumber,
                        PageSize = result.PageSize,
                        TotalItems = result.TotalItems
                    };

                    return View("Index", viewModel);
                }

                return StatusCode((int)response.StatusCode, "Errore nel recupero delle segnalazioni");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Errore: {ex.Message}";
                return View(new PaginatedSegnalazioniRegularViewModel());
            }
        }

        /// <summary>
        /// Ottieni una segnalazione regolare basata sul suo ID.
        /// </summary>
        /// <param name="segnalazioneRegularId">L'ID della segnalazione regolare</param>
        /// <returns>Ritorna la vista con i dettagli della segnalazione</returns>
        [HttpGet]
        public async Task<IActionResult> GetSegnalazioneAnonimaById(int Id)
        {
            try
            {
                // Chiamata all'API backend per ottenere la segnalazione tramite il suo ID
                var response = await _client.GetAsync($"{baseAddress}/SegnalazioniAnonime/getSegnalazioneAnonimaById/{Id}");

                if (response.IsSuccessStatusCode)
                {
                    // Deserializza la risposta JSON in un oggetto SegnalazioneRegularView
                    var segnalazione = await response.Content.ReadFromJsonAsync<SegnalazioneAnonimaView>();

                    if (segnalazione != null)
                    {
                        // Ritorna la vista con i dettagli della segnalazione
                        return View("DettaglioSegnalazionAnonima", segnalazione);
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

    }
}
