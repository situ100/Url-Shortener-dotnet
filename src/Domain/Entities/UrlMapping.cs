namespace Domain.Entities
{
    // Reprezentuje mapowanie URL w systemie skracania URL
    public class UrlMapping
    {
        // Unikalny identyfikator mapowania URL
        public int Id { get; set; }
        //Oryginalny, długi URL
        public string OriginalUrl { get; set; } = default!;
        //Skrócony URL (slug)
        public string Slug { get; set; } = default!;
        //Data utworzenia mapowania URL
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        //Data wygaśnięcia mapowania URL (jeśli dotyczy)
        public DateTime? ExpiresAt { get; set; }

        //Denormalizacja - przechowuje liczbę kliknięć
        public long TotalClicks { get; set; }

        //Nawigacyjne właściwości
        public ICollection<Click> Clicks { get; set; } = new List<Click>();
    }
}