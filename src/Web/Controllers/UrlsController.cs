using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

// Kontroler do zarządzania URL-ami
public class UrlsController : Controller
{
    private readonly IUrlService _urls;
    private readonly ILogger<UrlsController> _log;
    private readonly LinkGenerator _links;
    // Konstruktor do inicijalizacji serwisu URL i loggera
    public UrlsController(IUrlService urls, ILogger<UrlsController> log, LinkGenerator links)
    {
        _urls = urls;
        _log = log;
        _links = links;
    }

    // Akcja do wyświetlania listy URL-i
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var list = await _urls.GetAllAsync(ct);
        return View(list);
    }
    public IActionResult Create() => View();


    // Akcja do tworzenia nowego skróconego URL-a
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string originalUrl, CancellationToken ct)
    {
        // Walidacja wejściowego URL-a
        if (string.IsNullOrWhiteSpace(originalUrl))
        {
            ModelState.AddModelError("originalUrl", "The URL is required.");
            return View();
        }
        try
        // Próba utworzenia skróconego URL-a
        {
            var created = await _urls.CreateAsync(originalUrl, ct);

            var shortUrl = $"{Request.Scheme}://{Request.Host}/{created.Slug}";
            TempData["ShortUrl"] = shortUrl;
            return RedirectToAction(nameof(Details), new { id = created.Id });
        }
        // Obsługa wyjątków i wyświetlanie błędów
        catch (Exception ex)
        {
            _log.LogWarning(ex, "Create URL failed");
            ModelState.AddModelError(nameof(originalUrl), ex.Message);
            return View();
        }
    }
    // Akcja do wyświetlania szczegółów skróconego URL-a
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var entity = await _urls.GetByIdAsync(id, ct);
        if (entity == null)
        {
            return NotFound();
        }
        var stats = await _urls.GetCountryStatsAsync(id, ct);
        ViewBag.CountryStats = stats; // lista (Country, Count)

        ViewBag.ShortUrl = $"{Request.Scheme}://{Request.Host}/{entity.Slug}";
        return View(entity);
    }
    // Akcja do generowania i zwracania kodu QR dla skróconego URL-a
    [HttpGet]
    public async Task<IActionResult> Qr(int id, [FromServices] IQrCodeService qr, CancellationToken ct)
    {
        var entity = await _urls.GetByIdAsync(id, ct);
        if (entity == null)
            return NotFound();
        var shortUrl = $"{Request.Scheme}://{Request.Host}/{entity.Slug}";
        var pngBytes = qr.GeneratePng(shortUrl,8);
        return File(pngBytes, "image/png");
    }
}