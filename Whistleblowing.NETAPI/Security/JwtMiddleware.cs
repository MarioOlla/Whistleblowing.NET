using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

public class JwtMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IConfiguration _configuration;

	public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
	{
		_next = next;
		_configuration = configuration;
	}

	public async Task Invoke(HttpContext context)
	{
		var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

		if (token != null && !context.Request.Path.ToString().EndsWith("/login"))
		{
			AttachUserToContext(context, token);
		}

		await _next(context);
	}

	private void AttachUserToContext(HttpContext context, string token)
	{
		try
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

			tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = false,
				ValidateAudience = false,
				ClockSkew = TimeSpan.Zero // Disabilita la tolleranza temporale di default di 5 minuti
			}, out SecurityToken validatedToken);

			var jwtToken = (JwtSecurityToken)validatedToken;
			var userId = jwtToken.Claims.First(x => x.Type == "UserId").Value;

			// Imposta il contesto di sicurezza con l'utente autenticato
			var claims = new[] {
				new Claim(ClaimTypes.Name, userId)
			};

			var identity = new ClaimsIdentity(claims, "Jwt");
			context.User = new ClaimsPrincipal(identity);
		}
		catch (Exception)
		{
			// Se la validazione del token fallisce, non impostiamo il contesto utente
		}
	}
}
