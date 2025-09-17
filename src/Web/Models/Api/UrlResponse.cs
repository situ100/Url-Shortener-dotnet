namespace Web.Models.Api;

public class UrlResponse
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortUrl { get; set; } = string.Empty;
    public long TotalClicks { get; set; }
}

