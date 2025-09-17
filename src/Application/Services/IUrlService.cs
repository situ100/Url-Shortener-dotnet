using Domain.Entities;
namespace Application.Services;

// Servis dla zarządzania skracaniem i mapowaniem URL-i
public interface IUrlService
{
    // Tworzy nowe mapowanie URL
    Task<UrlMapping> CreateAsync(string originalUrl, CancellationToken ct = default);
    // Pobiera mapowanie URL na podstawie jego unikalnego identyfikatora
    Task<UrlMapping?> GetByIdAsync(int id, CancellationToken ct = default);
    // Pobiera mapowanie URL na podstawie jego skrótu (slug)
    Task<UrlMapping?> GetBySlugAsync(string slug, CancellationToken ct = default);
    // Pobiera wszystkie mapowania URL
    Task<List<UrlMapping>> GetAllAsync(CancellationToken ct = default);
    // Inkrementuje licznik kliknięć dla danego mapowania URL
    Task IncrementClickAsync(int urlId, CancellationToken ct = default);
    // Rejestruje kliknięcie z dodatkowymi informacjami, takimi jak IP klienta i kod kraju
    Task RecordClickAsync(int urlId, string? clientIp, string? countryCode = null, CancellationToken ct = default);
    // Pobiera statystyki kliknięć dla danego mapowania URL
    Task<IReadOnlyList<(string Country, long Count)>> GetCountryStatsAsync(int urlId, CancellationToken ct = default);

}