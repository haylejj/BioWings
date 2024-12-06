using BioWings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioWings.Persistence.Configurations;

public class GenusConfiguration : IEntityTypeConfiguration<Genus>
{
    public void Configure(EntityTypeBuilder<Genus> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(x => x.Species)
            .WithOne(x => x.Genus)
            .HasForeignKey(x => x.GenusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

