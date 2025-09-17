namespace Domain.Entities;

public class Click
{
    // Unikalny identyfikator kliknięcia
    public int Id { get; set; }
    // Identyfikator powiązanego mapowania URL
    public int UrlMappingId { get; set; }
    // Data i czas kliknięcia w formacie UTC
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    // Adres IP klienta, który wykonał kliknięcie
    public string? ClientIp { get; set; }
    // Kod kraju na podstawie adresu IP klienta
    public string? CountryCode { get; set; }

    public UrlMapping UrlMapping { get; set; } = default!;
}