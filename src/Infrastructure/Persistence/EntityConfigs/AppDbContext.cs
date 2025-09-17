using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

// Kontekst bazy danych aplikacji
public class AppDbContext : DbContext
{
    // Konstruktor przyjmujący opcje kontekstu
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSety reprezentujące tabele w bazie danych
    public DbSet<UrlMapping> UrlMappings => Set<UrlMapping>();
    public DbSet<Click> Clicks => Set<Click>();

    // Konfiguracja modelu bazy danych
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}
