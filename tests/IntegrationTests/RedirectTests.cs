using System.Net;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Xunit;

namespace IntegrationTests;

public class RedirectTests : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory _factory;

    public RedirectTests(TestWebAppFactory factory) => _factory = factory;

    [Fact]
    public async Task Go_Redirects_And_Increments_Clicks()
    {
        // Arrange: seed w bazie
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();

            db.UrlMappings.Add(new UrlMapping
            {
                OriginalUrl = "https://example.com/",
                Slug = "Test123",
                CreatedAtUtc = DateTime.UtcNow,
                TotalClicks = 0
            });
            await db.SaveChangesAsync();
        }

        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false // chcemy zobaczyć 302 i Location
        });

        // Act
        var resp = await client.GetAsync("/Test123");

        // Assert: 302 + Location nagłówek
        Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
        Assert.Equal("https://example.com/", resp.Headers.Location!.ToString());

        // Sprawdź licznik w DB
        using var scope2 = _factory.Services.CreateScope();
        var db2 = scope2.ServiceProvider.GetRequiredService<AppDbContext>();
        var entity = await db2.UrlMappings.AsNoTracking().SingleAsync(u => u.Slug == "Test123");
        Assert.Equal(1, entity.TotalClicks);

    }
}