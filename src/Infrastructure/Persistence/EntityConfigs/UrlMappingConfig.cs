using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigs;

// Konfiguracja encji UrlMapping dla Entity Framework
public class UrlMappingConfig : IEntityTypeConfiguration<UrlMapping>
{
    // Metoda konfigurująca encję UrlMapping
    public void Configure(EntityTypeBuilder<UrlMapping> b)
    {
        // Mapowanie na tabelę "UrlMappings"
        b.ToTable("UrlMappings");
        b.HasKey(x => x.Id);

        // Oryginalny, długi URL musi być wymagany i ma maksymalną długość 2048 znaków
        b.Property(x => x.OriginalUrl)
            .IsRequired()
            .HasMaxLength(2048);
        
        // Skrócony URL (slug) musi być unikalny i ma maksymalną długość 16 znaków
        b.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(16);

        // Data utworzenia mapowania URL jest wymagana
        b.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Indeksy dla optymalizacji wyszukiwania
        b.HasIndex(x => x.Slug)
            .IsUnique();
        b.HasIndex(x => x.OriginalUrl);

        // Konfiguracja relacji jeden-do-wielu z encją Click
        b.HasMany(x => x.Clicks)
            .WithOne(c => c.UrlMapping)
            .HasForeignKey(x => x.UrlMappingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}