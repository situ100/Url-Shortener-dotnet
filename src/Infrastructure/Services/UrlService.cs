using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

// Implementacja serwisu dla zarządzania skracaniem i mapowaniem URL-i
public class UrlService : IUrlService
{
    private readonly AppDbContext _dbContext;
    private readonly ISlugService _slugService;


    public UrlService(AppDbContext dbContext, ISlugService slugService)
    {
        _dbContext = dbContext;
        _slugService = slugService;
    }
    // Tworzy nowe mapowanie URL
    public async Task<UrlMapping> CreateAsync(string originalUrl, CancellationToken ct = default)
    {
        // Walidacja URL
        if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out var uriResult)
            || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Invalid URL format", nameof(originalUrl));
        }
        // Generowanie unikalnego skrótu (slug)
        string slug;
        int attempts = 0;
        do
        {
            slug = _slugService.GenerateSlug(7);
            attempts++;
        }
        // Próbuj maksymalnie 5 razy, aby znaleźć unikalny slug
        while (attempts < 5 && await _dbContext.UrlMappings.AnyAsync(u => u.Slug == slug, ct));
        if (await _dbContext.UrlMappings.AnyAsync(u => u.Slug == slug, ct))
        {
            throw new InvalidOperationException("Failed to generate a unique slug after multiple attempts.");
        }
        var entity = new UrlMapping
        {
            OriginalUrl = originalUrl,
            Slug = slug,
            CreatedAtUtc = DateTime.UtcNow,
            TotalClicks = 0
        };

        _dbContext.UrlMappings.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return entity;
    }
    public Task<UrlMapping?> GetByIdAsync(int id, CancellationToken ct = default) => _dbContext.UrlMappings
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == id, ct);
    public Task<UrlMapping?> GetBySlugAsync(string slug, CancellationToken ct = default) => _dbContext.UrlMappings.FirstOrDefaultAsync(u => u.Slug == slug, ct);

    public Task<List<UrlMapping>> GetAllAsync(CancellationToken ct = default) => _dbContext.UrlMappings.AsNoTracking().OrderByDescending(u => u.CreatedAtUtc).ToListAsync(ct);

    public async Task IncrementClickAsync(int urlId, CancellationToken ct = default)
    {
        var entity = await _dbContext.UrlMappings.FirstOrDefaultAsync(u => u.Id == urlId, ct);
        if (entity == null) return;
        entity.TotalClicks++;
        await _dbContext.SaveChangesAsync(ct);
    }
    // Rejestruje kliknięcie z dodatkowymi informacjami, takimi jak IP klienta i kod kraju
    public async Task RecordClickAsync(int urlId, string? clientIp, string? countryCode = null, CancellationToken ct = default)
    {
        var click = new Click
        {
            UrlMappingId = urlId,
            CreatedAtUtc = DateTime.UtcNow,
            ClientIp = clientIp,
            CountryCode = countryCode
        };
        _dbContext.Clicks.Add(click);

        var entity = await _dbContext.UrlMappings.FirstOrDefaultAsync(u => u.Id == urlId, ct);
        if (entity != null)
        {
            entity.TotalClicks++;
        }
        await _dbContext.SaveChangesAsync(ct);
    }
    public async Task<IReadOnlyList<(string Country, long Count)>> GetCountryStatsAsync(int urlId, CancellationToken ct = default)
    {
        var q = _dbContext.Clicks
        .AsNoTracking()
        .Where(c => c.UrlMappingId == urlId && c.CountryCode != null)
        .GroupBy(c => c.CountryCode!)
        .Select(g => new { Country = g.Key, Count = g.LongCount() })
        .OrderByDescending(x => x.Count);

    var data = await q.ToListAsync(ct);
    return data.Select(x => (x.Country, x.Count)).ToList();
    }
    

    
}