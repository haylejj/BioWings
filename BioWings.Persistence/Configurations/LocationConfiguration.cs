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
            .HasMaxLength(50);

        builder.Property(x => x.Latitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.Longitude)
            .HasPrecision(10, 6)
            .IsRequired();

        builder.Property(x => x.XCoord)
            .HasPrecision(18, 6);

        builder.Property(x => x.YCoord)
            .HasPrecision(18, 6);

        builder.Property(x => x.Altitude1)
            .HasPrecision(8, 2);

        builder.Property(x => x.Altitude2)
            .HasPrecision(8, 2);

        builder.Property(x => x.SquareRef)
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

