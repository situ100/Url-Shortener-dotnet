using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Services;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();     // potrzebne dla Swaggera (także dla Minimal APIs)
builder.Services.AddSwaggerGen();               // generator OpenAPI

//Rejestracja servisów
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseNpgsql(connectionString));

// Rejestracja serwisu do zarządzania URL-ami
builder.Services.AddScoped<IUrlService, UrlService>();

// Rejestracja serwisu do generowania slug-ów
builder.Services.AddScoped<ISlugService, SlugService>();

// Rejestracja serwisu do generowania kodów QR
builder.Services.AddScoped<IQrCodeService, QrCodeService>();

// Rejestracja serwisu do geolokalizacji
builder.Services.AddSingleton<IGeoService, GeoService>();

var app = builder.Build();

// Swagger – włączamy ZAWSZE, niezależnie od środowiska
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Dajemy chociaż jeden endpoint (Minimal API), żeby Swagger miał co pokazać
app.MapGet("/api/ping", () => Results.Ok(new { message = "pong" }));


// 1) redirect dla slugów
app.MapControllerRoute(
    name: "redirect",
    pattern: "{slug}",
    defaults: new { controller = "Redirects", action = "Go" });

// 2) domyślna dla całej reszty (MVC + widoki)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Urls}/{action=Create}/{id?}")
    .WithStaticAssets();

// (opcjonalnie, jeśli będziesz mieć kontrolery API z atrybutami)
app.MapControllers();

app.Run();

public partial class Program { } // dla testów integracyjnych
