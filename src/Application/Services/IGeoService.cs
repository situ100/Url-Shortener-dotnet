namespace Application.Services;

public interface IGeoService
{
    // Metoda do rozwiązywania kodu kraju na podstawie adresu IP
    string? ResolveCountryCode(string? ip);
}