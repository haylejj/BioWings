using BioWings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioWings.Persistence.Configurations;

public class ObservationConfiguration : IEntityTypeConfiguration<Observation>
{
    public void Configure(EntityTypeBuilder<Observation> builder)
    {
        builder.HasKey(x => x.Id);


        builder.Property(x => x.Sex)
            .HasMaxLength(15);

        builder.Property(x => x.ObservationDate)
            .IsRequired();


        builder.Property(x => x.LifeStage)
            .HasMaxLength(50);

        builder.Property(x => x.Notes)
            .HasMaxLength(200);

        builder.Property(x => x.Source)
            .HasMaxLength(100);

        builder.HasIndex(x => x.SpeciesId);
        builder.HasIndex(x => x.LocationId);
        builder.HasIndex(x => x.ObserverId);

    }
}

