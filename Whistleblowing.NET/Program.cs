using Microsoft.EntityFrameworkCore;
using Whistleblowing.NET.Data;

var builder = WebApplication.CreateBuilder(args);


// Aggiungi il contesto del database
builder.Services.AddDbContext<WhistleBlowingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WhistleBlowingContext")));


// Add services to the container.
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
