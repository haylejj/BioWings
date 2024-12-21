using BioWings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioWings.Persistence.Configurations;

public class SpeciesConfiguration : IEntityTypeConfiguration<Species>
{
    public void Configure(EntityTypeBuilder<Species> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ScientificName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Name)
            .HasMaxLength(70);

        builder.Property(x => x.EUName)
            .HasMaxLength(70);

        builder.Property(x => x.FullName)
            .HasMaxLength(100);

        builder.Property(x => x.TurkishName)
            .HasMaxLength(70);

        builder.Property(x => x.EnglishName)
            .HasMaxLength(70);

        builder.Property(x => x.TurkishNamesTrakel)
            .HasMaxLength(70);

        builder.Property(x => x.Trakel)
            .HasMaxLength(70);

        builder.Property(x => x.KocakName)
            .HasMaxLength(70);

        builder.Property(x => x.HesselbarthName)
            .HasMaxLength(70);

        builder.HasMany(x => x.Subspecies)
            .WithOne(x => x.Species)
            .HasForeignKey(x => x.SpeciesId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Observations)
            .WithOne(x => x.Species)
            .HasForeignKey(x => x.SpeciesId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ScientificName, x.GenusId, x.AuthorityId });

        builder.HasIndex(x => x.Name);

        builder.HasIndex(x => x.ScientificName);

        builder.HasIndex(x => x.GenusId);

        builder.HasIndex(x => x.AuthorityId);
    }
}

