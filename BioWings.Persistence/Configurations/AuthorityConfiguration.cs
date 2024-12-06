using BioWings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioWings.Persistence.Configurations;

public class AuthorityConfiguration : IEntityTypeConfiguration<Authority>
{
    public void Configure(EntityTypeBuilder<Authority> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Year)
            .IsRequired();

        builder.HasMany(x => x.Species)
            .WithOne(x => x.Authority)
            .HasForeignKey(x => x.AuthorityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

