using System.Security.Cryptography.X509Certificates;
using Application.Services;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;

namespace Infrastructure.Services;

public class GeoService : IGeoService, IDisposable
{
    private readonly DatabaseReader? _reader;

    public GeoService()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Geo", "GeoLite2-Country.mmdb");
        if (File.Exists(path))
        
            _reader = new DatabaseReader(path);
    }
        public string? ResolveCountryCode(string? ip)
    {
        if (_reader is null || string.IsNullOrWhiteSpace(ip)) return null;
        try
        {
            CountryResponse resp = _reader.Country(ip);
            return resp?.Country?.IsoCode; //np PL,DE itp
        }
        catch
        {
            return null;
        }
    }
    public void Dispose() => _reader?.Dispose();
    }