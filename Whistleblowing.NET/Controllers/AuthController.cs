using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Whistleblowing.NET.Models;
using Whistleblowing.NET.Models.DTO;

namespace Whistleblowing.NET.Controllers
{
    public class AuthController : Controller
    {
        private readonly JwtHelper _jwtHelper;
        private readonly HttpClient _client;
        private readonly Uri baseAddress = new Uri("https://localhost:44300/api");
        private readonly IHttpContextAccessor _contextAccessor;



        public AuthController(IHttpContextAccessor _contextAccs, JwtHelper jwtHelper)
        {
            _jwtHelper = jwtHelper;
            _client = new HttpClient() { BaseAddress = baseAddress };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _contextAccessor = _contextAccs;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UtenteDTORegister user)
        {
            if (ModelState.IsValid)
            {

                var jsonUser = System.Text.Json.JsonSerializer.Serialize(user);
                var content = new StringContent(jsonUser, Encoding.UTF8, "application/json");

                // Invia la richiesta POST al backend per la registrazione dell'utente
                var response = await _client.PostAsync(baseAddress + $"/Auth/Register", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Registrazione avvenuta con successo!";
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Si è verificato un errore durante la registrazione.");
                    return View(user);
                }
            }

            return View(user);
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UtenteDTOLogin user)
        {
            if (ModelState.IsValid)
            {
                var jsonUser = System.Text.Json.JsonSerializer.Serialize(user);
                var content = new StringContent(jsonUser, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync($"{baseAddress}/Auth/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddMinutes(30)
                    };
                    Response.Cookies.Append("jwtToken", tokenResponse.Token, cookieOptions);
                    string jwtToken = tokenResponse.Token;

                    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var jwtSecurityToken = tokenHandler.ReadJwtToken(jwtToken);

                    var claims = jwtSecurityToken.Claims.ToDictionary(c => c.Type, c => c.Value);

                    if (bool.TryParse(claims.GetValueOrDefault("HasChangedPassword"), out bool parsedHashChangePass))
                    {
                        tokenResponse.HasChangedPassword = parsedHashChangePass;
                    }
                    if (tokenResponse.HasChangedPassword)
                    {
                        return RedirectToAction("ChangePassword", "Auth");
                    }
                    //if (claims.GetValueOrDefault("Ruolo").Equals("Amministratore"))
                    //{
                    //    return RedirectToAction("Index", "Amministration");
                    //}
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Errore_Login"] = "E-mail o password errati";
                    return View(user);
                }

            }
            return View(user);
        }


    }
}
