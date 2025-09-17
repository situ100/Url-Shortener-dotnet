using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Web.Models.Api;

namespace Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class UrlsApiController : ControllerBase
{
    private readonly IUrlService _urls;
    private readonly IQrCodeService _qr;
    private readonly ILogger<UrlsApiController> _log;

    public UrlsApiController(IUrlService urls, IQrCodeService qr, ILogger<UrlsApiController> log)
    {
        _urls = urls;
        _qr = qr;
        _log = log;
    }

    //Utwórz nowy skrót do podanego URL.
    [HttpPost]
    [ProducesResponseType(typeof(UrlResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUrlRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.OriginalUrl))
            return BadRequest(new { error = "OriginalUrl is required." });

        try
        {
            var created = await _urls.CreateAsync(req.OriginalUrl, ct);
            var shortUrl = $"{Request.Scheme}://{Request.Host}/{created.Slug}";

            var resp = new UrlResponse
            {
                Id = created.Id,
                Slug = created.Slug,
                OriginalUrl = created.OriginalUrl,
                ShortUrl = shortUrl,
                TotalClicks = created.TotalClicks
            };

            // Location nagłówek do szczegółów:
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, resp);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = "InvalidUrl", message = ex.Message });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Create URL failed");
            return Problem("Internal error");
        }
    }

    //Pobierz szczegóły skrótu.
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UrlResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var entity = await _urls.GetByIdAsync(id, ct);
        if (entity is null) return NotFound();

        var shortUrl = $"{Request.Scheme}://{Request.Host}/{entity.Slug}";
        var resp = new UrlResponse
        {
            Id = entity.Id,
            Slug = entity.Slug,
            OriginalUrl = entity.OriginalUrl,
            ShortUrl = shortUrl,
            TotalClicks = entity.TotalClicks
        };
        return Ok(resp);
    }

    //PNG z kodem QR dla skrótu.
    [HttpGet("{id:int}/qr")]
    [Produces("image/png")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQr([FromRoute] int id, CancellationToken ct)
    {
        var entity = await _urls.GetByIdAsync(id, ct);
        if (entity is null) return NotFound();

        var shortUrl = $"{Request.Scheme}://{Request.Host}/{entity.Slug}";
        var png = _qr.GeneratePng(shortUrl, 8);
        return File(png, "image/png");
    }
}
