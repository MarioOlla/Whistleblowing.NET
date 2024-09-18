using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Whistleblowing.NET.Data;

var builder = WebApplication.CreateBuilder(args);

// Aggiungi i servizi al container.
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Aggiungi il contesto del database
builder.Services.AddDbContext<WhistleBlowingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WhistleBlowingContext")));

// Configura l'autenticazione basata su cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Auth/Login"; // Percorso alla pagina di login
		options.LogoutPath = "/Auth/Logout"; // Percorso alla pagina di logout
		options.AccessDeniedPath = "/Home/AccessDenied"; // Pagina di accesso negato
		options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Durata del cookie
		options.Cookie.HttpOnly = true; // Protezione da attacchi XSS
		options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Imposta su Always per HTTPS
		options.Cookie.SameSite = SameSiteMode.Strict; // Protezione da CSRF
		options.SlidingExpiration = true; // Rigenera il cookie prima della scadenza per estendere la sessione
	});


// Aggiungi i controller e le viste
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

// Aggiungi il middleware di rate limiting
app.UseIpRateLimiting();

app.UseStaticFiles();

app.UseRouting();

// Attiva l'autenticazione e autorizzazione
app.UseAuthentication(); // Deve venire prima di UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
