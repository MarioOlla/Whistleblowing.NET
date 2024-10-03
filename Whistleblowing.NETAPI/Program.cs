using Whistleblowing.NETAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Whistleblowing.NETAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);


	builder.Services.AddDbContext<WhistleBlowingContext>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("WhistleBlowingContext") ?? throw new InvalidOperationException("Connection string 'WhistleBlowingContext' not found.")));
	builder.Services.AddIdentity<User, IdentityRole>(options =>
	{
		options.Password.RequireDigit = true;
		options.Password.RequireLowercase = true;
		options.Password.RequireUppercase = true;
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequiredLength = 6;


	})
		.AddEntityFrameworkStores<WhistleBlowingContext>()
		.AddDefaultTokenProviders();



// Aggiungi il contesto del database
builder.Services.AddDbContext<WhistleBlowingContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("WhistleBlowingContext")));

// servizio web e applicazione che effettua richieste hanno domini differenti
// quindi, abilitando CORS, consento ad altri domini di chiamare l'API
builder.Services.AddCors(options =>
	options.AddDefaultPolicy(policy =>
		policy.WithOrigins("http://localhost:44300")
		.AllowAnyOrigin()
		.AllowAnyMethod()
		.AllowAnyHeader()
		));
builder.Services.AddCors(options =>
	options.AddDefaultPolicy(policy =>
		policy.WithOrigins("http://localhost:44316")
		.AllowAnyOrigin()
		.AllowAnyMethod()
		.AllowAnyHeader()
		));




builder.Services.AddControllers().AddJsonOptions(x =>
	x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	   .AddCookie(options =>
	   {

		   options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
		   options.Cookie.Name = "Cookie"; // Specifica il nome del cookie
										   //options.Cookie.HttpOnly = true;
										   //options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Imposta la politica di sicurezza del cookie
										   //options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None; // Imposta SameSite su Strict per proteggere da attacchi CSRF
		   options.Cookie.IsEssential = true; // Imposta il cookie come essenziale per le richieste
		   options.AccessDeniedPath = "/Unauthorized/ErrorPage"; // Imposta il percorso di accesso negato per il reindirizzamento
		   options.LoginPath = "/Auth/Login";
	   });

//
// Add services to the container.
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});





// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.CustomSchemaIds(type => type.FullName);

}); ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
