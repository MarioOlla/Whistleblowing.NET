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
        }
}
