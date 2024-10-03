using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Whistleblowing.NET.Models;

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

		public IActionResult Index()
		{
			return View();
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
	}
}



	

