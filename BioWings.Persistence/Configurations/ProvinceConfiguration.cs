using BioWings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioWings.Persistence.Configurations;

public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(70);

        builder.Property(x => x.ProvinceCode)
            .IsRequired();

        builder.HasMany(x => x.Locations)
            .WithOne(x => x.Province)
            .HasForeignKey(x => x.ProvinceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

