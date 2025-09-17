using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Persistence.EntityConfigs;
//Konfiguracja encji Click dla Entity Framework
public class ClickConfig : IEntityTypeConfiguration<Click>
{
    public void Configure(EntityTypeBuilder<Click> b)
    {
        // Mapowanie na tabelę "Clicks"
        b.ToTable("Clicks");
        b.HasKey(x => x.Id);

        // Konfiguracja właściwości
        b.Property(x => x.ClientIp).HasMaxLength(45);
        b.Property(x => x.CountryCode).HasMaxLength(2);
        // Data i czas kliknięcia jest wymagana
        b.Property(x => x.CreatedAtUtc).IsRequired();
        // Indeksy dla optymalizacji wyszukiwania
        b.HasIndex(x => new { x.UrlMappingId, x.CreatedAtUtc });
    }
}