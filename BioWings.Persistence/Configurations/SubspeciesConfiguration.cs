using BioWings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioWings.Persistence.Configurations;

public class SubspeciesConfiguration : IEntityTypeConfiguration<Subspecies>
{
    public void Configure(EntityTypeBuilder<Subspecies> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(70);
    }
}

