using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
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
            try
            {
                var response = await _client.GetAsync($"{baseAddress}/SegnalazioniRegular/GetAllSegnalazioniRegularTotali?pageNumber={pageNumber}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserializza l'oggetto che contiene i dati e i metadati di paginazione
                    var result = System.Text.Json.JsonSerializer.Deserialize<PaginatedResponse<PaginatedSegnalazioniRegularViewModel>>(jsonResponse, options);

                    var viewModel = new PaginatedSegnalazioniRegularViewModel
                    {
                        SegnalazioniRegulars = result.Data,
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

        // Classe helper per gestire la risposta paginata
        public class PaginatedResponse<T>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalItems { get; set; }
            public List<T> Data { get; set; }
        }



        [HttpGet]
        public IActionResult PostSegnalazioneRegular()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> PostSegnalazioneRegular(SegnalazioneRegularDTOInserimento segnalazione)
        {
            // Verifica la validità del modello
            if (!ModelState.IsValid)
            {
                return View(segnalazione); 
            }

            try
            {
                // Recupera il token JWT dall'header
                string jwt = Request.Cookies["jwtToken"];

                if (string.IsNullOrEmpty(jwt))
                {
                    ModelState.AddModelError(string.Empty, "Token non trovato. Accedi nuovamente.");
                    return View(segnalazione);
                }

                // Configura un oggetto JSON per la richiesta
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                };
                var requestContent = new StringContent(JsonConvert.SerializeObject(segnalazione), Encoding.UTF8, "application/json");

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                var response = await _client.PostAsync($"{baseAddress}/SegnalazioniRegular/PostSegnalazioneRegular", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    string ruoloClaim = null;

                    // Provo a recuperare la claim "Ruolo" dal contesto User
                    ruoloClaim = User.Claims.FirstOrDefault(c => c.Type == "Ruolo")?.Value;

                    // Se non trovato, decodifico manualmente il token JWT
                    if (string.IsNullOrEmpty(ruoloClaim))
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var token = handler.ReadJwtToken(jwt);
                        ruoloClaim = token.Claims.FirstOrDefault(c => c.Type == "Ruolo")?.Value;
                        Console.WriteLine($"RuoloClaim decodificato: {ruoloClaim}");
                    }

                    // Processo il ruolo trovato
                    if (!string.IsNullOrEmpty(ruoloClaim) && Enum.TryParse<Ruolo>(ruoloClaim, out var userRole))
                    {
                        if (userRole == Ruolo.UTENTE)
                        {
                            TempData["SuccessMessage"] = "Segnalazione inviata con successo!";
                            return RedirectToAction("GetMySegnalazioniRegular");
                        }
                        else if (userRole == Ruolo.OPERATORE)
                        {
                            TempData["SuccessMessage"] = "Segnalazione inviata con successo!";
                            return RedirectToAction("Index");
                        }
                    }

                    // Ruolo sconosciuto o non trovato
                    Console.WriteLine("RuoloClaim è null o il valore non è valido.");
                    TempData["SuccessMessage"] = "Segnalazione inviata con successo!";
                    return RedirectToAction("Login");
                }

                // Gestione di errori restituiti dall'API
                var errorDetails = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Errore API: {errorDetails}");
                return View(segnalazione);
            }
            catch (Exception ex)
            {
                // Log dell'eccezione (può essere sostituito da un logger se disponibile)
                Console.WriteLine($"Errore durante la richiesta: {ex.Message}");

                // Mostra un errore generico nella vista
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante l'invio della segnalazione.");
                return View(segnalazione);
            }
        }











        ///// <summary>
        ///// Ottiene tutte le segnalazioni indipendentemente dall'utente.
        ///// </summary>
        ///// <param name="pageNumber">Numero della pagina da visualizzare</param>
        ///// <param name="pageSize">Numero di elementi per pagina</param>
        ///// <returns>Risultato paginato con tutte le segnalazioni</returns>
        //[HttpGet("GetAllSegnalazioniRegularTotali")]
        //public async Task<IActionResult> GetAllSegnalazioniRegularTotali(int pageNumber = 1, int pageSize = 10)
        //{
        //    try

        //    {

        //        // Chiamata all'endpoint del backend
        //        var response = await _client.GetAsync($"{baseAddress}/SegnalazioniRegular/GetAllSegnalazioniRegularTotali?pageNumber={pageNumber}&pageSize={pageSize}");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            // Leggi i dati della risposta
        //            var data = await response.Content.ReadFromJsonAsync<dynamic>(); // Usa dynamic per il debug
        //            if (data != null)
        //            {
        //                return Ok(data); // Ritorna i dati ricevuti
        //            }
        //        }

        //        // Se la risposta non è andata a buon fine
        //        return StatusCode((int)response.StatusCode, "Errore nel recupero delle segnalazioni.");
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        // Gestisci le eccezioni di rete
        //        return StatusCode(500, $"Errore di rete: {ex.Message}");
        //    }
        //}

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
        /// Ottiene le segnalazioni regular dell'utente loggato
        /// </summary>
        /// <returns>Una vista con le segnalazioni dell'utente loggato</returns>
        [HttpGet("GetMySegnalazioniRegular")]
        public async Task<IActionResult> GetMySegnalazioniRegular(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                string token = Request.Cookies["jwtToken"];

                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account"); // Reindirizza alla pagina di login
                }

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _client.GetAsync($"{baseAddress}/SegnalazioniRegular/GetMySegnalazioniRegular?pageNumber={pageNumber}&pageSize={pageSize}");

                Console.WriteLine(response.IsSuccessStatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserializza l'oggetto che contiene i dati e i metadati di paginazione
                    var result = System.Text.Json.JsonSerializer.Deserialize<PaginatedResponse<PaginatedSegnalazioniRegularViewUtente>>(jsonResponse, options);

                    Console.WriteLine(result.ToString());

                    var viewModel = new PaginatedSegnalazioniRegularViewUtente
                    {
                        SegnalazioniRegulars = result.Data,
                        PageNumber = result.PageNumber,
                        PageSize = result.PageSize,
                        TotalItems = result.TotalItems
                    };

                    Console.WriteLine(viewModel.ToString());

                    return View("SegnalazioniPerUtente", viewModel);

                }

                return StatusCode((int)response.StatusCode, "Errore nel recupero delle segnalazioni");
            }
            catch (HttpRequestException ex)
            {

                return StatusCode(500, $"Errore di rete: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> ModificaSegnalazioneRegular(int id)
        {
            // Prepara l'URL per chiamare l'API
            string url = $"{baseAddress}/SegnalazioniRegular/segnalazioneModifica/{id}"; // Sostituisci baseAddress con l'URL del backend

            // Esegui la richiesta GET
            var response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound(); // Se l'API non restituisce un successo, mostra NotFound
            }

            // Deserializza la risposta JSON in un oggetto DTO usando Newtonsoft.Json
            var jsonString = await response.Content.ReadAsStringAsync();
            var segnalazione = JsonConvert.DeserializeObject<SegnalazionRegularDTOModifica>(jsonString);

            return View(segnalazione);
        }




        [HttpPost]
        public async Task<IActionResult> ModificaSegnalazioneReg(SegnalazionRegularDTOModifica segnalazione)
        {

            Console.WriteLine("sono qui");
            // Verifica che i dati siano validi
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Dati della segnalazione non validi.");
                return View(segnalazione);
            }

            try
            {
                // Recupera il token JWT dai cookie
                string jwt = Request.Cookies["jwtToken"];
                if (string.IsNullOrEmpty(jwt))
                {
                    ModelState.AddModelError(string.Empty, "Token non trovato. Accedi nuovamente.");
                    return View(segnalazione);
                }

                // Recupera l'ID utente dalla claim del token JWT
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    ModelState.AddModelError(string.Empty, "ID utente non valido nel token. Accedi nuovamente.");
                    return View(segnalazione);
                }

                // Imposta l'URL dell'API per la modifica
                string url = $"{baseAddress}/SegnalazioniRegular/PutSegnalazioneRegular?userid={userId}";

                // Configura il contenuto della richiesta
                var requestContent = new StringContent(JsonConvert.SerializeObject(segnalazione), Encoding.UTF8, "application/json");

                // Aggiunge l'header di autorizzazione
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                // Invia la richiesta PUT all'API
                var response = await _client.PostAsync(url, requestContent);

                // Controlla la risposta dell'API
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Modifica della segnalazione completata con successo!";
                    return RedirectToAction("Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    ModelState.AddModelError(string.Empty, "Accesso negato: solo gli utenti con codice OPERATORE possono modificare le segnalazioni!");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ModelState.AddModelError(string.Empty, "Utente o segnalazione non trovati.");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Errore durante la modifica della segnalazione: {errorMessage}");
                }

                return View(segnalazione);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Errore durante la chiamata all'API: {ex.Message}");
                return View(segnalazione);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Errore imprevisto: {ex.Message}");
                return View(segnalazione);
            }
        }


    }
}
    






	

