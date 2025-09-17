# URL Shortener (.NET 8 + PostgreSQL + MVC + Swagger)

Prosty projekt typu URL Shortener napisany w **.NET 8**, z wykorzystaniem:
- ASP.NET Core MVC (UI + API)
- Entity Framework Core + PostgreSQL
- Docker (Postgres + aplikacja)
- Swagger (OpenAPI)
- QR code (QRCoder)
- Testy (xUnit, InMemory, WebApplicationFactory)

## Funkcje
- Skracanie linków (zabezpieczenie przed błędnymi URL)
- Przekierowania (`/{slug}`)
- Licznik kliknięć + zapis IP/kraju (GeoLite2)
- Generowanie kodów QR
- API (Swagger):
  - `POST /api/urls` – utwórz skrót
  - `GET /api/urls/{id}` – pobierz szczegóły
  - `GET /api/urls/{id}/qr` – QR code

## Struktura projektu
src/
Web/ -> ASP.NET Core MVC + API
Infrastructure/ -> EF Core, serwisy
Application/ -> Interfejsy, logika
Domain/ -> Encje
tests/
UnitTests/ -> testy jednostkowe
IntegrationTests/ -> testy integracyjne
docker/
docker-compose.yml

## Uruchomienie 
```bash
docker compose -f docker/docker-compose.yml up -d
dotnet ef database update -p src/Infrastructure -s src/Web
dotnet run --project src/Web

## Testy
dotnet test
