using Microsoft.AspNetCore.Mvc;
using Application.Services;

namespace Web.Controllers;

public class RedirectsController : Controller
{
    private readonly IUrlService _urls;
    private readonly IGeoService _geo;


    public RedirectsController(IUrlService urls, IGeoService geo)
    {
        _urls = urls;
        _geo = geo;
    }

    // Akcja do przekierowywania na oryginalny URL na podstawie slug-a
    [HttpGet]
    public async Task<IActionResult> Go(string slug, CancellationToken ct)
    {
        var entity = await _urls.GetBySlugAsync(slug, ct);
        if (entity == null)
            return NotFound("URL not found");

    // Pobierz IP klienta
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var country = _geo.ResolveCountryCode(ip);
        
        await _urls.RecordClickAsync(entity.Id, ip, country, ct);
        return Redirect(entity.OriginalUrl);    
    }
} 