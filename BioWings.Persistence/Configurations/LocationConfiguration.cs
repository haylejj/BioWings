using BioWings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioWings.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.SquareRef)
            .HasMaxLength(50);

        builder.Property(x => x.Latitude)
            .HasMaxLength(50);

        builder.Property(x => x.Longitude)
            .HasMaxLength(50);

        builder.Property(x => x.Altitude1)
            .HasMaxLength(50);

        builder.Property(x => x.Altitude2)
            .HasMaxLength(50);

        builder.Property(x => x.UtmReference)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasMany(x => x.Observations)
            .WithOne(x => x.Location)
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

