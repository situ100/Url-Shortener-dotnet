using System.Net;
using System.Net.Http.Json;
using Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Web.Models.Api;
using Xunit;

namespace IntegrationTests;

public class UrlsApiTests : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory _factory;

    public UrlsApiTests(TestWebAppFactory factory) => _factory = factory;

    [Fact]
    public async Task Create_Returns201_AndPersists()
    {
        var client = _factory.CreateClient();

        var payload = new { originalUrl = "https://example.com/abc" };
        var resp = await client.PostAsJsonAsync("/api/urls", payload);

        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);

        var dto = await resp.Content.ReadFromJsonAsync<UrlResponse>();
        Assert.NotNull(dto);
        Assert.True(dto!.Id > 0);
        Assert.Equal("https://example.com/abc", dto.OriginalUrl);
        Assert.False(string.IsNullOrWhiteSpace(dto.Slug));
        Assert.Contains($"/{dto.Slug}", dto.ShortUrl);

        // weryfikacja w bazie InMemory
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var saved = db.UrlMappings.Single(u => u.Id == dto.Id);
        Assert.Equal(dto.Slug, saved.Slug);
    }
}
