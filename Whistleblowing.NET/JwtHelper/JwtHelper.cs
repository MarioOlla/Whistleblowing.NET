using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Whistleblowing.NET.Models;

namespace Whistleblowing.NET
{
    public class JwtHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Questo metodo si occupa dell' estrazione dell' utente dal jwt contenuto nel cookie
        /// </summary>
        /// <returns>oggetto Utente</returns>
        public User GetUserFromJwt()
        {
            try
            {
                string jwtToken = _httpContextAccessor.HttpContext.Request.Cookies["jwtToken"];

                if (string.IsNullOrEmpty(jwtToken))
                {
                    return null;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "http://localhost:44300",
                    ValidAudience = "http://localhost:44316",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("UnaStringaSegretaComplessaEGenerataCasualmente12345!"))

                };

                var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out SecurityToken validatedToken);
                var jwtSecurityToken = validatedToken as JwtSecurityToken;

                if (jwtSecurityToken == null)
                {
                    return null;
                }

                var claims = jwtSecurityToken.Claims.ToDictionary(c => c.Type, c => c.Value);
                var utente = new User
                {
                    Id = int.Parse(claims["UtenteId"]),
                    Nome = claims["Nome"],
                    Cognome = claims["Cognome"],
                    Email = claims["sub"],
                    Ruolo = Enum.Parse<Ruolo>(claims["Ruolo"])

                };

                return utente;
            }
            catch (Exception ex)
            {
                //stampo l'eccezione sulla console
                Console.WriteLine(ex.ToString());
                return null;
            }

        }
    }
}
