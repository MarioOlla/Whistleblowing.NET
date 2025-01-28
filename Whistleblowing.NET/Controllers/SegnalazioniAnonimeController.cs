using Microsoft.AspNetCore.Mvc;
using static Whistleblowing.NET.Controllers.SegnalazioniRegularController;
using System.Text.Json;
using Whistleblowing.NET.Models;
using System.Net.Http.Headers;
using Whistleblowing.NET.Models.DTO;
using Newtonsoft.Json;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

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

        public IActionResult Cerca()
        {
            return View();
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

        [HttpGet]
        public async Task<IActionResult> GetSegnalazioneAnonimaById(int Id)
        {
            try
            {
                // Costruisco l'URL con il parametro query
                var response = await _client.GetAsync($"{baseAddress}/SegnalazioniAnonime/getSegnalazioneAnonimaById?segnalazioneAnonimaId={Id}");

                if (response.IsSuccessStatusCode)
                {
                    // Deserializzo la risposta JSON in un oggetto SegnalazioneAnonimaView
                    var segnalazione = await response.Content.ReadFromJsonAsync<SegnalazioneAnonimaView>();

                    if (segnalazione != null)
                    {
                        // Ritorna la vista con i dettagli della segnalazione
                        return View("DettaglioSegnalazioneAnonima", segnalazione);
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






        [HttpGet]
        public IActionResult PostSegnalazioneAnonima()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> PostSegnalazioneAnonima(SegnalazioneAnonimaDTO segnalazioneAnonima)
        {
            if (!ModelState.IsValid)
            {
                return View(segnalazioneAnonima);
            }
            try
            {
                //Recupero il token jwt dall' header
                string jwt = Request.Cookies["jwtToken"];



                if (string.IsNullOrEmpty(jwt))
                {
                    ModelState.AddModelError(string.Empty, "Token non trovato. Accedi nuovamente");
                    return View(segnalazioneAnonima);
                }

                //Configuro un oggetto json per la richiesta
                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                };

                var requestContent = new StringContent(JsonConvert.SerializeObject(segnalazioneAnonima), Encoding.UTF8, "application/json");

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                var response = await _client.PostAsync($"{baseAddress}/SegnalazioniAnonime", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    string ruoloClaim = null;

                    //Provo a recuperare la claim "Ruolo dal contesto User
                    ruoloClaim = User.Claims.FirstOrDefault(c => c.Type == "Ruolo")?.Value;

                    //Se non trovato, decodifico manualmente il token JWT

                    if (string.IsNullOrEmpty(ruoloClaim))
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var token = handler.ReadJwtToken(jwt);
                        ruoloClaim = token.Claims.FirstOrDefault(c => c.Type == "Ruolo")?.Value;
                        Console.WriteLine($"RuoloClaim decodificato: {ruoloClaim}");
                    }


                    //Processo il ruolo trovato
                    if (!string.IsNullOrEmpty(ruoloClaim) && Enum.TryParse<Ruolo>(ruoloClaim, out var userRole))
                    {
                        if (userRole == Ruolo.UTENTE)
                        {
                            TempData["SuccessMessage"] = "Segnalazione inviata con successo!";
                            return RedirectToAction("GetMySegnalazioniAnonima");
                        }
                        else if (userRole == Ruolo.OPERATORE)
                        {
                            TempData["SuccessMessage"] = "Segnalazione inviata con successo!";
                            return RedirectToAction("Index");
                        }
                    }

                    //Ruolo sconosciuto o non trovato
                    Console.WriteLine("RuoloClaim è null oppure il valore non è valido");
                    TempData["SuccessMessage"] = "Segnalazione inviata con successo!";
                    return RedirectToAction("Login");


                }

                //Gestione di errori restituiti dall' API
                var errorDetails = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Errore API: {errorDetails}");
                return View(segnalazioneAnonima);

            }
            catch (Exception ex)
            {
                //Log eccezione
                Console.WriteLine($"Errore durante la richiesta: {ex.Message}");

                //Mostra un errore generico nella vista
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante l' invio della segnalazione.");
                return View(segnalazioneAnonima);
            }

        }

        [HttpGet]
        public async Task<IActionResult> DecryptUserHashed(string userHashed, string pwd)
        {
            if (string.IsNullOrWhiteSpace(userHashed) || string.IsNullOrWhiteSpace(pwd))
            {
                ViewBag.ErrorMessage = "Entrambi i campi sono obbligatori.";
                return View("Errore");
            }

            try
            {
                var response = await _client.GetAsync($"{baseAddress}/SegnalazioniAnonime/DecryptUserHashed?userHashed={userHashed}&pwd={pwd}");

                if (response.IsSuccessStatusCode)
                {
                    var decryptedData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(decryptedData);

                    return View("UserHashed", result);
                }
                else
                {
                    ViewBag.ErrorMessage = await response.Content.ReadAsStringAsync();
                    return View("Errore");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Errore durante il processo di decrittazione: {ex.Message}";
                return View("Errore");
            }


        }

        [HttpGet]
        public async Task<IActionResult> ModificaSegnalazioneAnonima(int id)
        {
            // Prepara l'URL per chiamare l'API
            string url = $"{baseAddress}/SegnalazioniAnonime/segnalazioneAnonimaModifica/{id}"; // Sostituisci baseAddress con l'URL del backend

            // Esegui la richiesta GET
            var response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound(); // Se l'API non restituisce un successo, mostra NotFound
            }

            // Deserializza la risposta JSON in un oggetto DTO usando Newtonsoft.Json
            var jsonString = await response.Content.ReadAsStringAsync();
            var segnalazione = JsonConvert.DeserializeObject<SegnalazioneAnonimaDTOModifica>(jsonString);

            return View(segnalazione);
        }

        [HttpPost]
        public async Task<IActionResult> ModificaSegnalazioneAnonimaAzione(SegnalazioneAnonimaDTOModifica segnalazione)
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
                string url = $"{baseAddress}/SegnalazioniAnonime/PutSegnalazioneAnonima?userid={userId}";

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
