using Whistleblowing.NETAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Whistleblowing.NETAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Whistleblowing.NETAPI.Crypto;
using System.Security.Cryptography;
using Whistleblowing.NETAPI.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


//##################################################################### DB CONTEXT ########################################################################################


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



//#################################################################### CORS ##############################################################################################

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


//################################################################ JSON WEB TOKEN ###########################################################################################

// Configura l'autenticazione JWT
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
	};
});

builder.Services.AddScoped<JwtUtils>();


//
// Add services to the container.
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});



//############################################################# SWAGGER ##################################################################################################

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.CustomSchemaIds(type => type.FullName);

});



builder.Services.AddTransient<CryptoService>();

//############################################################ EMAIL SERVICE #############################################################################################

//configurazione email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Registrazione dei servizi
builder.Services.AddTransient<IEmailService, EmailService>();

//########################################################## PDF SERVICE #################################################################################################


//registro il servizio pdf
builder.Services.AddScoped<PdfService>();



var app = builder.Build();


//########################################################## CRYPTO SERVICE ##############################################################################################



using (var scope = app.Services.CreateScope())
{
    // Risolvi il servizio scoped (CryptoService)
    var cryptoService = scope.ServiceProvider.GetRequiredService<CryptoService>();

    try
    {

        CryptoKey cryptoData = cryptoService.fetchCryptoInfo();

        string passwordAdmin = "lallero";

        if (cryptoData == null)
        {
            cryptoData = CryptoService.GenerateCryptoData(passwordAdmin, 10000, 256);
            cryptoService.saveCryptoInfo(cryptoData);
            Console.WriteLine("INFO ******> Ho creato una nuova configurazione per la crittografia");
        }

        CryptoService.PrintCryptoDataToConsole(cryptoData);

        string msg = "Mio messaggio non cifrato";

        Console.WriteLine("INFO ******> Inizio encrypt dei dati");
        byte[] msgC = CryptoService.EncryptWithRSA(CryptoService.LoadPublicKey(cryptoData.RsaPublicKey), msg);
        Console.WriteLine("INFO ******> Fine encrypt dei dati");
        //stampa

        RSAParameters pk = CryptoService.LoadPrivateKey(passwordAdmin, cryptoData.EncryptedRsaPrivateKey, cryptoData.Salt, cryptoData.AesIterations,cryptoData.AesKeySize);

        string msgD = CryptoService.DecryptWithRSA(pk, msgC);

        Console.WriteLine("Messaggio decrittato: " + msgD);
    }
    catch (Exception ex)
    {
        // Logga o gestisci l'errore se qualcosa va storto
        Console.WriteLine("Errore durante il setup della crittografia: {0} \n\n {1}",ex.Message, ex.StackTrace);
    }
}




//########################################################### CONFIGURAZIONI DI AVVIO SERVIZI ############################################################################

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.UseMiddleware<JwtMiddleware>();


app.UseHttpsRedirection();


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
