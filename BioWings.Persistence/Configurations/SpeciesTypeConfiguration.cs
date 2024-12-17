using BioWings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioWings.Persistence.Configurations;

public class SpeciesTypeConfiguration : IEntityTypeConfiguration<SpeciesType>
{
    public void Configure(EntityTypeBuilder<SpeciesType> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(70);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

    }
}

